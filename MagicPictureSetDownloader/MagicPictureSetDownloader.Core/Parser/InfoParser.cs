using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MagicPictureSetDownloader.Core
{
    internal static class InfoParser
    {
        internal static IEnumerable<SetInfo> ParseSetsList(string htmltext)
        {
            return new SetParser().Parse(htmltext);
        }
        internal static IEnumerable<string> ParseCardUrls(string htmltext)
        {
            return new CardUrlParser().Parse(htmltext);
        }
        internal static CardInfo ParseCardInfo(string htmltext)
        {
            return new CardParser().Parse(htmltext).FirstOrDefault();
        }

        internal static string ExtractContent(string htmltext, string start, string end)
        {
            htmltext = WebUtility.HtmlDecode(htmltext);
            if (string.IsNullOrEmpty(start))
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
