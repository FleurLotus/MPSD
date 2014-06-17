using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MagicPictureSetDownloader.Core
{
    internal class InfoParser
    {
        private static readonly Regex _setRegex = new Regex(@"<a h?ref=""(?<setpath>(?<set>.{3})/.*\.html)""><img (?:[^>]*)src=""(?<setpictureurl>[^""]*)""(?:[^>]*)/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _nameRegex = new Regex(@"<title>(?<name>[^|>]*) \| [^|>]*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _pictureRegex = new Regex(@"<a href=""(?<cardpath>[^""]*\.html)""><img (?:[^>]*)src=""(?<pictureurl>[^""]*)""(?:[^>]*)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string StartContent = "<!--CONTENT-->";
        private const string EndContent = "<!--END CONTENT-->";

        internal IEnumerable<SetInfo> ParseSetsList(string htmltext)
        {
            string content = ExtractContent(htmltext);

            return _setRegex.Matches(content).Cast<Match>()
                                              .Select(m => new SetInfo(m.Groups["set"].Value, m.Groups["setpictureurl"].Value, m.Groups["setpath"].Value))
                                              .ToArray();
        }

        private string ExtractContent(string htmltext)
        {
            int startIndex = htmltext.LastIndexOf(StartContent, StringComparison.Ordinal);
            if (startIndex < 0)
                return htmltext;
            
            startIndex += StartContent.Length;

            int endIndex = htmltext.IndexOf(EndContent, startIndex, StringComparison.Ordinal);
            if (endIndex < 0)
                return htmltext;

            return htmltext.Substring(startIndex, endIndex - startIndex);
        }

        internal string ParseName(string htmltext)
        {
            Match m = _nameRegex.Match(htmltext);
            if (m.Success)
                return m.Groups["name"].Value;

            return null;
        }

        internal PictureInfo[] ParsePicturesList(string htmltext)
        {
            string content = ExtractContent(htmltext);

            return _pictureRegex.Matches(content).Cast<Match>()
                                                 .Select(m => new PictureInfo(m.Groups["cardpath"].Value, m.Groups["pictureurl"].Value))
                                                 .ToArray();

        }
    }
}
