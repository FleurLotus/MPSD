namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Text.RegularExpressions;
    using System.Xml;

    internal static class SymbolParser
    {
        private static readonly Regex _symbolRegex = new Regex(@"Image\.ashx\?size=(medium|small)&name=(?<symbol>.+)&type=symbol", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string Parse(IAwareXmlTextReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "img")
            {
                string src = reader.GetAttribute("src");
                Match m = _symbolRegex.Match(src);
                if (m.Success)
                    return m.Groups["symbol"].Value;
            }

            throw new ParserException("Can't retrieve symbol");
        }
    }
}
