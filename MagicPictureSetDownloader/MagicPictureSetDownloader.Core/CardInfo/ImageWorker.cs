namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;

    internal class ImageWorker : ICardInfoParserWorker
    {
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_cardImage for normal card
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl03_cardImage for part A of multi part card
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl04_cardImage for part B of multi part card
        private const string KeyStart = "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent";
        private const string KeyEnd = "cardimage";

        public bool WorkOnCurrentAtStart
        {
            get { return true; }
        }
        public static bool IsWorkingInfo(string id)
        {
            if (id == null)
                return false;

            string lid = id.ToLowerInvariant();

            return (lid.StartsWith(KeyStart) && lid.EndsWith(KeyEnd));
        }
        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            if (xmlReader.Name == "img")
            {
                string id = xmlReader.GetAttribute("id");
                if (IsWorkingInfo(id))
                {
                    string source = xmlReader.GetAttribute("src");
                    if (string.IsNullOrWhiteSpace(source))
                        throw new ParserException("Can't find image path");

                    return new Dictionary<string, string> { { CardParser.ImageKey, source } };
                }
            }
            return new Dictionary<string, string>();
        }
    }
}