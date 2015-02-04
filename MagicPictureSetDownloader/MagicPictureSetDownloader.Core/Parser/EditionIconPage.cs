namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class EditionIconPage : IParser<string>
    {
        private const string Start = @"<!-- Column 1 start -->";
        private const string Start2 = @"<b>Symbol:</b>";
        private const string End = "<!-- Column 1 end -->";

        private static readonly Regex _urlRegex = new Regex(@"<img\s*src=""(?<url>[^""]+)""[^<>]*(<|>)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public IEnumerable<string> Parse(string text)
        {
            string newtext = Parser.ExtractContent(text, Start, End, true, false);
            newtext = Parser.ExtractContent(newtext + End, Start2, End, true, false);

            Match match = _urlRegex.Match(newtext);
            if (!match.Success)
                return new string[0];

            return new[] { match.Groups["url"].Value };
        }
    }
}
