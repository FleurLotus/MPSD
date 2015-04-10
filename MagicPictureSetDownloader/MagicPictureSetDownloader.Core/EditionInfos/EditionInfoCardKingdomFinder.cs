namespace MagicPictureSetDownloader.Core.EditionInfos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class EditionInfoCardKingdomFinder : EditionInfoFinderBase
    {
        private static readonly IDictionary<string, IList<EditionIconInfo>> _cache = new Dictionary<string, IList<EditionIconInfo>>();

        private const string Start = @"<!-- Column 2 start -->";
        private const string Start2 = @"Magic: the Gathering";
        private const string End = @"<!-- Column 2 end -->";

        private const string StartSubPage = @"<!-- Column 1 start -->";
        private const string Start2SubPage = @"<b>Symbol:</b>";
        private const string EndSubPage = "<!-- Column 1 end -->";


        private static readonly Regex _editionRegex = new Regex(@"<a href=""(?<url>[^""]+)"">(?<name>[^<]*)<br></a>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex _urlRegex = new Regex(@"<img\s*src=""(?<url>[^""]+)""[^<>]*(<|>)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public EditionInfoCardKingdomFinder(Func<string, string> getHtml)
            : base(getHtml)
        {
            Replace = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
                                                  {
                                                      { "&", "and" },
                                                      { "Sixth", "6th" },
                                                      { "Seventh", "7th" },
                                                      { "Eighth", "8th" },
                                                      { "Ninth", "9th" },
                                                      { "Tenth", "10th" },
                                                      { "Ravnica: City of Guilds", "Ravnica" },
                                                      { "Unlimited", "XXXX" }, //Do not get the wrong image
                                                      { " Anthology,", ":" },

                                                      { "(2014)", null },
                                                      { "Magic: The Gathering", null },
                                                      { "Edition", null },
                                                      { "Core Set", null },
                                                      { "Classic", null },
                                                      { "—", null },
                                                      { "-", null },
                                                      { "Magic", null },
                                                  };
        }

        protected override IList<EditionIconInfo> Parse(string url)
        {
            IList<EditionIconInfo> ret;

            if (_cache.TryGetValue(url, out ret))
                return ret;

            string text = GetHtml(url);
            string newtext = Parser.ExtractContent(text, Start, End, true, false);
            newtext =  Parser.ExtractContent(newtext + End, Start2, End, true, false);

            ret = _editionRegex.Matches(newtext).OfType<Match>()
                                                .Select(match => new EditionIconInfo(match.Groups["name"].Value.Trim(), match.Groups["url"].Value, null))
                                                .ToList();
            _cache.Add(url, ret);
            return ret;
        }

        protected override void GetIconUrl(EditionIconInfo editionIconInfo)
        {
            if (editionIconInfo == null)
                return;

            string htmltext = GetHtml(editionIconInfo.Url);
            string newtext = Parser.ExtractContent(htmltext, StartSubPage, EndSubPage, true, false);
            newtext = Parser.ExtractContent(newtext + End, Start2SubPage, EndSubPage, true, false);

            Match match = _urlRegex.Match(newtext);
            editionIconInfo.Url = match.Success ? match.Groups["url"].Value : null;
        }
    }
}
