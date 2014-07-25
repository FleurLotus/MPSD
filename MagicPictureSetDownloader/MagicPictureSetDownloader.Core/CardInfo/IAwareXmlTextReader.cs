namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;
    using System.Xml;

    internal interface IAwareXmlTextReader
    {
        XmlNodeType NodeType { get;  }
        bool IsEmptyElement { get; }
        string Name { get;  }
        string Value { get; }

        bool Read();
        IDictionary<string, string> GetAttributes();
        string GetAttribute(string key);
    }
}
