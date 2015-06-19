namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;
    using System.Xml;

    using Common.Libray.Extension;

    internal class TextRowWorker : ICardInfoParserWorker
    {
        public bool WorkOnCurrentAtStart
        {
            get { return false; }
        }
        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            if (xmlReader.Name == "div" && xmlReader.GetAttribute("class") == "value")
            {
                string value = string.Empty;
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "div" && 
                        xmlReader.GetAttribute("class") =="cardtextbox")
                    {
                        string text = WorkOnTextBox(new AwareXmlTextReader(xmlReader));
                        if (!string.IsNullOrWhiteSpace(text))
                            value += "\r\n" + text;
                    }
                }
                if (string.IsNullOrEmpty(value))
                    throw new ParserException("No Text element found in Element");

                return new Dictionary<string, string> {{CardParser.TextKey, value.HtmlTrim()}};
            }
            return new Dictionary<string, string>();
        }


        private string WorkOnTextBox(IAwareXmlTextReader xmlReader)
        {
            string value = string.Empty;
            while (xmlReader.Read())
            {
                string text = null;
                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    text = xmlReader.Value.HtmlTrim();
                }
                else if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "img")
                {
                    text = SymbolParser.Parse(xmlReader);
                }

                if (!string.IsNullOrWhiteSpace(text))
                {
                    value += " " + text;
                }
            }

            return value.HtmlTrim();
        }
    }
}