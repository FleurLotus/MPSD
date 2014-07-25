namespace Common.XML
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    public static class Helper
    {
        public static IDictionary<string, string> GetAttributes(this XmlTextReader reader)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (!reader.MoveToFirstAttribute())
                return ret;

            do
            {
                ret.Add(reader.Name, reader.Value);
            } while (reader.MoveToNextAttribute());

            reader.MoveToElement();

            return ret;
        }
        public static string HtmlTrim(this string source)
        {
            return source.Replace("&nbsp;"," ").Trim(new[] {' ', '\t', '\n', '\r'});
        }
    }
}
