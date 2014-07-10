using System.Collections.Generic;
using System.IO;
using System.Xml;
using Common.Libray;
using MagicPictureSetDownloader.Core.CardInfo;

namespace MagicPictureSetDownloader.Core
{
    internal class CardParser : IParser<IDictionary<string,string>>
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


        public IEnumerable<IDictionary<string, string>> Parse(string text)
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

            CheckInfos(infos);
            return new[] {infos};
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
            //ALERT: Do the check 
            //throw new NotImplementedException();
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
