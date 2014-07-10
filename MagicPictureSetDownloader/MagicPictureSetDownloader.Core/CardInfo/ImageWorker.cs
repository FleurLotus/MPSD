using System.Collections.Generic;

namespace MagicPictureSetDownloader.Core.CardInfo
{
    internal class ImageWorker : ICardInfoParserWorker
    {
        private const string Key = "ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_cardImage";

        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            if (xmlReader.Name == "img" && xmlReader.GetAttribute("id") == Key)
            {
                string source = xmlReader.GetAttribute("src");
                if (string.IsNullOrWhiteSpace(source))
                    throw new ParserException("Can't find image path");

                return new Dictionary<string, string> {{CardParser.ImageKey, source}};
            }
            return new Dictionary<string, string>();
        }
    }
}