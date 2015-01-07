namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Common.Libray;
    using Common.XML;
    using MagicPictureSetDownloader.Core.CardInfo;

    internal class CardParser : IParser<CardWithExtraInfo>
    {
        private const string Start = @"<!-- Card Details Table -->";
        private const string End = @"<!-- End Card Details Table -->";
        private const string SubCardStart = @"<table class=""cardDetails cardComponent""";

        public const string ImageKey = "Image";
        public const string NameKey = "Name";
        public const string ManaCostKey = "ManaCost";
        public const string CmcKey = "CMC";
        public const string TypeKey = "Type";
        public const string TextKey = "Text";
        public const string FlavorKey = "Flavor";
        public const string PTKey = "PT";
        public const string SetKey = "Set";
        public const string RarityKey = "Rarity";
        public const string NumberKey = "Number";
        public const string ArtistKey = "Artist";

        public IEnumerable<CardWithExtraInfo> Parse(string text)
        {
            //No decode because we need to do a special correction for XmlTextReader to be able to read HTML with javascript
            //For end because of multi part card
            string cutText = Parser.ExtractContent(text, Start, End, false, true);
            
            if (cutText.IndexOf(End, StringComparison.InvariantCulture) >= 0)
                return ManageMultiPartCards(text, cutText);

            //Case for normal card
            CardWithExtraInfo cardWithExtraInfo = GenerateCard(cutText);
            return new[] { cardWithExtraInfo };
        }

        private IEnumerable<CardWithExtraInfo> ManageMultiPartCards(string text, string cutText)
        {
            //Case for multi part card
            List<CardWithExtraInfo> cardWithExtraInfos = new List<CardWithExtraInfo>();
            foreach (string subinfo in  cutText.Split(new[] { End }, StringSplitOptions.RemoveEmptyEntries))
            {
                int index = subinfo.LastIndexOf(SubCardStart, StringComparison.InvariantCulture);
                if (index < 0)
                {
                    continue;
                }

                cardWithExtraInfos.Add(GenerateCard(subinfo.Substring(index)));
            }

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
                    SwapCards(cardWithExtraInfos);
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
                    ICardInfoParserWorker worker = CardInfoParserWorkerFactory.CreateParserWorker(reader);

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
                Name = infos.GetOrDefault(NameKey),
                CastingCost = infos.GetOrDefault(ManaCostKey),
                Text = infos.GetOrDefault(TextKey),
                Type = infos.GetOrDefault(TypeKey),
                PictureUrl = infos.GetOrDefault(ImageKey),
                Rarity = infos.GetOrDefault(RarityKey)
            };

            if (IsCreature(cardWithExtraInfo.Type))
            {
                string htmlTrim = infos.GetOrDefault(PTKey).HtmlTrim();
                cardWithExtraInfo.Power = GetPower(htmlTrim);
                cardWithExtraInfo.Toughness = GetToughness(htmlTrim);
            }
            if (IsPlaneswalker(cardWithExtraInfo.Type))
            {
                string htmlTrim = infos.GetOrDefault(PTKey).HtmlTrim();
                //Possible see CheckInfos for more info
                if (!string.IsNullOrWhiteSpace(htmlTrim))
                    cardWithExtraInfo.Loyalty = int.Parse(htmlTrim);
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
                throw new ParserException("No name found");
            if (!infos.ContainsKey(TypeKey))
                throw new ParserException("No type found");
            if (!infos.ContainsKey(RarityKey))
                throw new ParserException("No rarity found");
            
            string type = infos.GetOrDefault(TypeKey);
            string castingcost = infos.GetOrDefault(ManaCostKey);
            //Add check on casting cost because of second face of Garruk, the Veil-Cursed which has no loyalty counter
            if ((IsCreature(type) || IsPlaneswalker(type)) && !infos.ContainsKey(PTKey) && !string.IsNullOrWhiteSpace(castingcost))
                throw new ParserException("No PT/Loyalty found");
        }
        private string GetPower(string text)
        {
            return text.Split('/')[0].HtmlTrim();
        }
        private string GetToughness(string text)
        {
            return text.Split('/')[1].HtmlTrim();
        }
        private bool IsCreature(string type)
        {
            return type.ToLowerInvariant().Contains("creature") && !type.ToLowerInvariant().Contains("enchant creature");
        }
        private bool IsPlaneswalker(string type)
        {
            return type.ToLowerInvariant().Contains("planeswalker");
        }
        private string SpecialXMLCorrection(string text)
        {
            //Ensure unique root element because of fragment file
            text = "<root>" + text + "</root>";

            //For XmlTextReader which doesn't support & caracter
            text = text.Replace("&amp;", "&");
            text = text.Replace("&", "&amp;");
            text = text.Replace("<</i>", "&lt;</i>");

            //Known XML issues in file
            text = text.Replace("<div>", "<tr>");
            text = text.Replace("<i>", "");
            text = text.Replace("</i>", "");
            text = text.Replace("--->", "-->");

            return text;
        }
    }
}
