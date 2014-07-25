namespace MagicPictureSetDownloader.Core
{
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
            //ALERT: Manage double face card???
            text = Parser.ExtractContent(text, Start, End, false);

            IDictionary<string, string> infos = new Dictionary<string, string>();

            using (XmlTextReader xmlReader = new XmlTextReader(new StringReader(SpecialXMLCorrection(text))))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    IAwareXmlTextReader reader = new AwareXmlTextReader(xmlReader);
                    ICardInfoParserWorker worker = CardInfoParserWorkerFactory.CreateParserWorker(reader);

                    if (worker == null)
                        continue;

                    infos.AddRange(ParseElement(reader, worker));
                }
            }

            CardWithExtraInfo cardWithExtraInfo = GenerateCard(infos);
            cardWithExtraInfo.PictureUrl = infos.GetOrDefault(ImageKey);
            cardWithExtraInfo.Rarity = infos.GetOrDefault(RarityKey);

            return new[] {cardWithExtraInfo};
        }
        private CardWithExtraInfo GenerateCard(IDictionary<string, string> infos)
        {
            CheckInfos(infos);
            CardWithExtraInfo cardWithExtraInfo = new CardWithExtraInfo
            {
                Name = infos.GetOrDefault(NameKey),
                CastingCost = infos.GetOrDefault(ManaCostKey),
                Text = infos.GetOrDefault(TextKey),
                Type = infos.GetOrDefault(TypeKey)
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
                cardWithExtraInfo.Loyalty = int.Parse(htmlTrim);
            }

            cardWithExtraInfo.Type = infos.GetOrDefault(TypeKey);
            return cardWithExtraInfo;
        }
        private IDictionary<string, string> ParseElement(IAwareXmlTextReader xmlReader, ICardInfoParserWorker worker)
        {
            IDictionary<string, string> parsedInfo = new Dictionary<string, string>();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    IDictionary<string, string> workOnElement = worker.WorkOnElement(new AwareXmlTextReader(xmlReader));
                    parsedInfo.AddRange(workOnElement);
                }
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
                throw new ParserException("No ratiry found");
            
            string type = infos.GetOrDefault(TypeKey);

            if ((IsCreature(type) || IsPlaneswalker(type)) && !infos.ContainsKey(PTKey))
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
            return type.ToLowerInvariant().Contains("creature");
        }
        private bool IsPlaneswalker(string type)
        {
            return type.ToLowerInvariant().Contains("planeswalker");
        }
        private string SpecialXMLCorrection(string text)
        {
            //Ensure unique root element because of fragment file
            text = "<root>" + text + "</root>";

            //Known XML issues in file
            text = text.Replace("<div>", "<tr>");
            text = text.Replace("<i>", "");
            text = text.Replace("</i>", "");

            //For XmlTextReader which doesn't support & caracter
            text = text.Replace("&amp;", "&");
            text = text.Replace("&", "&amp;");
            return text;
        }
    }
}
