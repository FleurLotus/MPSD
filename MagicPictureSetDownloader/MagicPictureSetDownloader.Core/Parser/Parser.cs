namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;

    internal static class Parser
    {
        private static readonly Regex _idRegex = new Regex(@"multiverseid=(?<id>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static IEnumerable<EditionInfo> ParseEditionsList(string htmltext)
        {
            return new EditionParser().Parse(htmltext);
        }
        internal static IEnumerable<string> ParseCardUrls(string htmltext)
        {
            return new CardUrlParser().Parse(htmltext);
        }
        internal static IEnumerable<CardWithExtraInfo> ParseCardInfo(string htmltext)
        {
            return new CardParser().Parse(htmltext);
        }
        internal static IEnumerable<CardLanguageInfo> ParseCardLanguage(string htmltext)
        {
            return new CardLanguageParser().Parse(htmltext);
        }

        public static int ExtractIdGatherer(string pictureUrl)
        {
            Match m = _idRegex.Match(pictureUrl);
            if (!m.Success)
                throw new ParserException("Can't parse card picture Id");
            return int.Parse(m.Groups["id"].Value);
        }
        
        internal static string ExtractContent(string htmltext, string start, string end, bool withHtmlDecode, bool untilLastEnd)
        {
            if (withHtmlDecode)
                htmltext = WebUtility.HtmlDecode(htmltext);

            if (string.IsNullOrEmpty(start))
                return htmltext;

            int startIndex = htmltext.IndexOf(start, StringComparison.InvariantCulture);
            if (startIndex < 0)
                return htmltext;

            startIndex += start.Length;

            int endIndex = untilLastEnd ? htmltext.LastIndexOf(end, StringComparison.InvariantCulture) : htmltext.IndexOf(end, startIndex, StringComparison.InvariantCulture);
            if (endIndex < 0 || endIndex < startIndex)
                return htmltext;

            return htmltext.Substring(startIndex, endIndex - startIndex);
        }
    }
}
