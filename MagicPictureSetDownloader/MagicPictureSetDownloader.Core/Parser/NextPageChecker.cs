 namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class NextPageChecker
    {
        private static readonly Regex _pageRegex = new Regex(@"<a href=""[^""]+page=(?<page>\d+)[^""]+""(?: style=""text-decoration:underline;"")?>\d+</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public static void CheckHasNextPage(string text, bool expectedAtLeastOne)
        {
            MatchCollection matches = _pageRegex.Matches(text);
            if (matches.Count == 0)
            {
                if (expectedAtLeastOne)
                    throw new ParserException("Can't parse footer row");

                return;
            }

            HashSet<int> pages = new HashSet<int>();
            foreach (Match match in matches)
                pages.Add(int.Parse(match.Groups["page"].Value));

            throw new NextPageException(pages.ToArray());
        }
    }
}
