namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;
    using System.Xml;

    internal class ManaRowWorker : ICardInfoParserWorker
    {
        public bool WorkOnCurrentAtStart
        {
            get { return false; }
        }
        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            if (xmlReader.Name == "div" && xmlReader.GetAttribute("class") == "value")
            {
                string value = null;
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "img")
                    {
                        string symbol = SymbolParser.Parse(xmlReader);
                        if (string.IsNullOrEmpty(value))
                            value = symbol;
                        else
                            value += " " + symbol;
                    }
                }
                if (string.IsNullOrEmpty(value))
                    throw new ParserException("No Text element found in Element");

                return new Dictionary<string, string> {{CardParserBase.ManaCostKey, value}};
            }
            return new Dictionary<string, string>();
        }
    }
}