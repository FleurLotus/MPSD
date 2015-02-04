namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class EditionIconPageListParser: IParser<EditionIconInfo>
    {
        private const string Start = @"<!-- Column 2 start -->";
        private const string Start2 = @"Magic: the Gathering";
        private const string End = @"<!-- Column 2 end -->";

        private static readonly Regex _editionRegex = new Regex(@"<a href=""(?<url>[^""]+)"">(?<name>[^<]*)<br></a>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public IEnumerable<EditionIconInfo> Parse(string text)
        {
            string newtext = Parser.ExtractContent(text, Start, End, true, false);
            newtext =  Parser.ExtractContent(newtext + End, Start2, End, true, false);

            foreach (Match match in _editionRegex.Matches(newtext))
            {
                yield return new EditionIconInfo(match.Groups["name"].Value.Trim(), match.Groups["url"].Value);
            }
        }
    }
}
