namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;
    using System.Xml;

    internal class VariationsWorker : ICardInfoParserWorker
    {
        //cctl00_ctl00_ctl00_MainContent_SubContent_SubContent_variationLinks for normal card
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl03_variationLinks for part A of multi part card
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl04_variationLinks for part B of multi part card
        private const string KeyStart = "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent";
        private const string KeyEnd = "variationlinks";

        public const char Separator = ';';

        public bool WorkOnCurrentAtStart
        {
            get { return false; }
        }

        private bool IsWorkingInfo(string id)
        {
            if (id == null)
            {
                return false;
            }

            string lid = id.ToLowerInvariant();

            return (lid.StartsWith(KeyStart) && lid.EndsWith(KeyEnd));
        }

        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            string value = string.Empty;

            if (xmlReader.Name == "div")
            {
                string id = xmlReader.GetAttribute("id");
                if (IsWorkingInfo(id))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "a")
                        {
                            string text = xmlReader.GetAttribute("id");
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    value += Separator;
                                }

                                value += text;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new ParserException("No A element found in Element");
                }

                return new Dictionary<string, string> { { CardParserBase.VariationsKey, value } };
            }
            return new Dictionary<string, string>();
        }
    }
}