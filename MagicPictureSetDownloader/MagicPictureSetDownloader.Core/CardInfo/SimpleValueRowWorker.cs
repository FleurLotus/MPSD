namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;
    using System.Xml;

    using Common.Library.Extension;

    internal class SimpleValueRowWorker : ICardInfoParserWorker
    {
        private readonly string _key;

        public bool WorkOnCurrentAtStart
        {
            get { return false; }
        }

        public SimpleValueRowWorker(string key)
        {
            _key = key;
        }

        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            if (xmlReader.Name == "div" && xmlReader.GetAttribute("class") == "value")
            {
                string value = null;
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Text)
                    {
                        if (!string.IsNullOrEmpty(value))
                            throw new ParserException("Multiple Text element in Element");

                        value = xmlReader.Value.HtmlTrim();
                    }
                }
                if (string.IsNullOrEmpty(value))
                    throw new ParserException("No Text element found in Element for Key: " + _key);

                return new Dictionary<string, string> {{_key, value}};
            }
            return new Dictionary<string, string>();
        }
    }
}