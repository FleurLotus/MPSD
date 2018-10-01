namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class EditionParser : IParser<EditionInfo>
    {
        private const string Start = @"<select name=""ctl00$ctl00$MainContent$Content$SearchControls$setAddText""";
        private const string End = "</select>";

        private static readonly Regex _setRegex = new Regex(@"<option value=""(?<editionname>[^>]+)"">.*?</option>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex _searchPageRegex = new Regex(@"var cardSearchPage = '(?<url>[^']+)';", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public IEnumerable<EditionInfo> Parse(string text)
        {
            Match match = _searchPageRegex.Match(text);
            if (!match.Success)
            {
                throw new ParserException("Can't find seach page");
            }

            string url = match.Groups["url"].Value;

            string content = Parser.ExtractContent(text, Start, End, true, false);

            return _setRegex.Matches(content).Cast<Match>()
                .Select(m => new EditionInfo(m.Groups["editionname"].Value, url));
        }
    }
}
