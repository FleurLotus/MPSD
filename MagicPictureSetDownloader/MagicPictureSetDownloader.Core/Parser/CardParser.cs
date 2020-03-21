namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Text.RegularExpressions;

    using Common.Library.Extension;
    using MagicPictureSetDownloader.Core.CardInfo;

    internal class CardParser : CardParserBase, IParser<CardWithExtraInfo>
    {
        private static readonly Regex ParenthesesRegex = new Regex(@"\s+\(\w\)$", RegexOptions.Compiled);

        public IEnumerable<CardWithExtraInfo> Parse(string text)
        {
            string[] cutTexts = ExtractCardText(text);

            if (cutTexts.Length == 1)
            {
                //Case for normal card
                CardWithExtraInfo cardWithExtraInfo = GenerateCard(cutTexts[0]);
                return new[] { cardWithExtraInfo };
            }

            return ManageMultiPartCards(text, cutTexts);
        }

        private IEnumerable<CardWithExtraInfo> ManageMultiPartCards(string text, IEnumerable<string> cutTexts)
        {
            //Case for multi part card
            List<CardWithExtraInfo> cardWithExtraInfos = cutTexts.Select(GenerateCard).ToList();

            if (cardWithExtraInfos.Count != 2)
            {
                throw new ParserException("Wrong number of parsed cards in a single block of multipart card! Check HTML source");
            }

            cardWithExtraInfos[0].PartName = cardWithExtraInfos[0].Name;
            cardWithExtraInfos[1].PartName = cardWithExtraInfos[1].Name;
            cardWithExtraInfos[1].OtherPathName = cardWithExtraInfos[0].PartName;
            cardWithExtraInfos[0].OtherPathName = cardWithExtraInfos[1].PartName;

            string cardName = CardNameParser.Parse(text);

            //Manage first
            //Up/Down card - always the good way
            //Splitted card
            if (cardName != cardWithExtraInfos[0].Name && cardName != cardWithExtraInfos[1].Name)
            {
                cardWithExtraInfos[0].Name = cardName;
                cardWithExtraInfos[1].Name = cardName;

                if (cardName.StartsWith(cardWithExtraInfos[1].PartName))
                {
                    SwapCards(cardWithExtraInfos);
                }
            }
            //Recto Verso card
            else if (cardWithExtraInfos[0].CastingCost == null)
            {
                SwapCards(cardWithExtraInfos);
            }
            else if (cardWithExtraInfos[1].CastingCost == null)
            {
                //Do nothing
            }
            //MultiCard 
            else
            {
                //Do nothing
            }

            return cardWithExtraInfos;
        }
        private CardWithExtraInfo GenerateCard(string text)
        {
            IDictionary<string, string> infos = new Dictionary<string, string>();
            //Parsing
            using (XmlTextReader xmlReader = new XmlTextReader(new StringReader(SpecialXMLCorrection(text))))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    IAwareXmlTextReader reader = new AwareXmlTextReader(xmlReader);
                    ICardInfoParserWorker worker = CardInfoParserWorkerFactory.Instance.CreateParserWorker(reader);

                    if (worker == null)
                    {
                        continue;
                    }

                    infos.AddRange(ParseElement(reader, worker));
                }
            }
            //Check parsing result
            CheckInfos(infos);
            
            //Generate result class
            CardWithExtraInfo cardWithExtraInfo = new CardWithExtraInfo
            {
                Name = RemoveParentheses(infos.GetOrDefault(NameKey)),
                CastingCost = infos.GetOrDefault(ManaCostKey),
                Text = infos.GetOrDefault(TextKey),
                Type = infos.GetOrDefault(TypeKey),
                PictureUrl = infos.GetOrDefault(ImageKey),
                Rarity = infos.GetOrDefault(RarityKey)
            };

            if (MagicRules.IsCreature(cardWithExtraInfo.Type) || MagicRules.IsVehicle(cardWithExtraInfo.Type))
            {
                string htmlTrim = infos.GetOrDefault(PTKey).HtmlTrim();
                cardWithExtraInfo.Power = GetPower(htmlTrim);
                cardWithExtraInfo.Toughness = GetToughness(htmlTrim);
            }
            if (MagicRules.IsPlaneswalker(cardWithExtraInfo.Type))
            {
                string htmlTrim = infos.GetOrDefault(PTKey).HtmlTrim();
                //Possible see CheckInfos for more info
                if (!string.IsNullOrWhiteSpace(htmlTrim))
                {
                    htmlTrim = htmlTrim.ToUpper();
                    //Special case for:
                    //  Nissa, Steward of Elements with loyalty to X
                    //  B.O.B. (Bevy of Beebles) with loyalty to *
                    if (htmlTrim == "X" || htmlTrim == "*")
                    {
                        cardWithExtraInfo.Loyalty = htmlTrim;
                    }
                    else
                    {
                        cardWithExtraInfo.Loyalty = int.Parse(htmlTrim).ToString();
                    }
                }
            }

            cardWithExtraInfo.Type = infos.GetOrDefault(TypeKey);
            return cardWithExtraInfo;
        }
        private void SwapCards(IList<CardWithExtraInfo> cardWithExtraInfos)
        {
            CardWithExtraInfo temp = cardWithExtraInfos[0];
            cardWithExtraInfos[0] = cardWithExtraInfos[1];
            cardWithExtraInfos[1] = temp;
        }
        private IDictionary<string, string> ParseElement(IAwareXmlTextReader xmlReader, ICardInfoParserWorker worker)
        {
            IDictionary<string, string> parsedInfo = new Dictionary<string, string>();
            bool readOk = worker.WorkOnCurrentAtStart || xmlReader.Read();
            
            while (readOk)
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    IDictionary<string, string> workOnElement = worker.WorkOnElement(new AwareXmlTextReader(xmlReader));
                    parsedInfo.AddRange(workOnElement);
                }
                readOk = xmlReader.Read();
            }

            return parsedInfo;
        }
        private void CheckInfos(IDictionary<string, string> infos)
        {
            if (!infos.ContainsKey(NameKey))
            {
                throw new ParserException("No name found");
            }

            if (!infos.ContainsKey(TypeKey))
            {
                throw new ParserException("No type found");
            }

            if (!infos.ContainsKey(RarityKey))
            {
                throw new ParserException("No rarity found");
            }

            string type = infos.GetOrDefault(TypeKey);
            string castingcost = infos.GetOrDefault(ManaCostKey);
            //Add check on casting cost because of second face of Garruk, the Veil-Cursed which has no loyalty counter
            if ((MagicRules.IsCreature(type) || MagicRules.IsPlaneswalker(type) || MagicRules.IsVehicle(type)) && !infos.ContainsKey(PTKey) && !string.IsNullOrWhiteSpace(castingcost))
            {
                throw new ParserException("No PT/Loyalty found");
            }
        }
        private string GetPower(string text)
        {
            return text.Split('/')[0].HtmlTrim();
        }
        private string GetToughness(string text)
        {
            return text.Split('/')[1].HtmlTrim();
        }
        private string RemoveParentheses(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return ParenthesesRegex.Replace(name, string.Empty).HtmlTrim();
        }
    }
}
