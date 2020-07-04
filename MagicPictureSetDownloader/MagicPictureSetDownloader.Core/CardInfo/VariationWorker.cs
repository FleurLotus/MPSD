namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;
    using System.Xml;

    internal class VariationsWorker : ICardInfoParserWorker
    {
        public const char Separator = ';';

        public VariationsWorker(IAwareXmlTextReader xmlReader)
        {
            //
        }
        public bool WorkOnCurrentAtStart
        {
            get { return false; }
        }

        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            string value = string.Empty;

            if (xmlReader.Name == "div" && xmlReader.GetAttribute("id") == "ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_variationLinks")
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