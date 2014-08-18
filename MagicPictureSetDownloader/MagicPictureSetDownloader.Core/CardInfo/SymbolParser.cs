namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Text.RegularExpressions;
    using System.Xml;

    public static class SymbolParser
    {
        public const string Prefix = "@";

        private static readonly Regex _symbolRegex = new Regex(@"Image\.ashx\?size=(medium|small)&name=(?<symbol>.+)&type=symbol", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static string Parse(IAwareXmlTextReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "img")
            {
                string src = reader.GetAttribute("src");
                Match m = _symbolRegex.Match(src);
                if (m.Success)
                    return Prefix + m.Groups["symbol"].Value;
            }

            throw new ParserException("Can't retrieve symbol");
        }
    }
}
