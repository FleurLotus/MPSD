namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;

    internal static class Parser
    {
        private static readonly Regex _idRegex = new Regex(@"multiverseid=(?<id>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static IEnumerable<SetInfo> ParseSetsList(string htmltext)
        {
            return new SetParser().Parse(htmltext);
        }
        internal static IEnumerable<string> ParseCardUrls(string htmltext)
        {
            return new CardUrlParser().Parse(htmltext);
        }
        internal static CardWithExtraInfo ParseCardInfo(string htmltext)
        {
            return new CardParser().Parse(htmltext).FirstOrDefault();
        }
        public static int ExtractIdGatherer(string pictureUrl)
        {
            Match m = _idRegex.Match(pictureUrl);
            if (!m.Success)
                throw new ParserException("Can't parse card picture Id");
            return int.Parse(m.Groups["id"].Value);
        }
        
        internal static string ExtractContent(string htmltext, string start, string end, bool withHtmlDecode)
        {
            if (withHtmlDecode)
                htmltext = WebUtility.HtmlDecode(htmltext);

            if (String.IsNullOrEmpty(start))
                return htmltext;

            int startIndex = htmltext.LastIndexOf(start, StringComparison.Ordinal);
            if (startIndex < 0)
                return htmltext;

            startIndex += start.Length;

            int endIndex = htmltext.IndexOf(end, startIndex, StringComparison.Ordinal);
            if (endIndex < 0)
                return htmltext;

            return htmltext.Substring(startIndex, endIndex - startIndex);
        }
    }
}
