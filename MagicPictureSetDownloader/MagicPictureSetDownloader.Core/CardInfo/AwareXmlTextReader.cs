namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Common.XML;

    internal class AwareXmlTextReader : IAwareXmlTextReader
    {
        private readonly XmlTextReader _reader;
        private readonly IAwareXmlTextReader _parent;
        private int _level = 1;
        private readonly string _sourceElementName;

        public AwareXmlTextReader(XmlTextReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                throw new ArgumentException("Can create AwareXmlTextReader only on Element");
            }

            _level = reader.IsEmptyElement ? 0 : 1; 
            _sourceElementName = reader.Name;
            _reader = reader;
        }
        public AwareXmlTextReader(IAwareXmlTextReader parent)
        {
            _sourceElementName = parent.Name;
            _parent = parent;
        }
        
        public XmlNodeType NodeType
        {
            get { return _parent != null ? _parent.NodeType : _reader.NodeType; }
        }
        public bool IsEmptyElement
        {
            get { return  _parent != null ? _parent.IsEmptyElement : _reader.IsEmptyElement; }
        }
        public string Name
        {
            get { return  _parent != null ? _parent.Name : _reader.Name; }
        }
        public string Value
        {
            get { return _parent != null ? _parent.Value : _reader.Value; }
        }

        public bool Read()
        {
            if (_level <= 0)
            {
                return false;
            }

            bool ret =  _parent != null ? _parent.Read() : _reader.Read();
            if (ret)
            {
                if (NodeType == XmlNodeType.Element)
                {
                    if (!IsEmptyElement)
                    {
                        _level++;
                    }
                }

                if (NodeType == XmlNodeType.EndElement)
                {
                    _level--;
                }

                if (_level == 0 && _sourceElementName != Name)
                {
                    throw new ParserException("Closing Element is not matching opening one");
                }
            }

            return ret;
        }
        public IDictionary<string, string> GetAttributes()
        {
            return _parent != null ? _parent.GetAttributes() : _reader.GetAttributes(); 
        }
        public string GetAttribute(string key)
        {
            return _parent != null ? _parent.GetAttribute(key) : _reader.GetAttribute(key); 
        }
    }
}
