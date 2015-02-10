namespace MagicPictureSetDownloader.Core.EditionInfos
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Common.Libray;
    using Common.Libray.Html;

    internal class EditionInfoWikiaFinder : EditionInfoFinderBase
    {
        private static readonly IDictionary<string, IList<EditionIconInfo>> _cache = new Dictionary<string, IList<EditionIconInfo>>();

        private const string Start = @"id=""All_sets""";
        private const string End = @"</table>";
        private const string UrlStart = @"<noscript>";
        private const string UrlEnd = @"</noscript>";
        private static readonly Regex _urlRegex = new Regex(@"<img src=""(?<url>[^""]+)""", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public EditionInfoWikiaFinder(Func<string, string> getHtml)
            : base(getHtml)
        {
            Replace = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
                                                  {
                                                      { "Fourth", "4th" },
                                                      { "Fifth", "5th" },
                                                      { "Sixth", "6th" },
                                                      { "Seventh", "7th" },
                                                      { "Eighth", "8th" },
                                                      { "Ninth", "9th" },
                                                      { "Tenth", "10th" },
                                                      { " Anthology,", ":" },
                                                      { "Portal Second Age", "Portal The Second Age" },

                                                      { "Box Set", null },
                                                      { "Magic: The Gathering", null },
                                                      { "Edition", null },
                                                      { "Core Set", null },
                                                      { "Classic", null },
                                                      { "—", null },
                                                      { "-", null },
                                                  };

        }

        protected override IList<EditionIconInfo> Parse(string url)
        {
            IList<EditionIconInfo> ret;
            if (_cache.TryGetValue(url, out ret))
                return ret;

            string text = GetHtml(url);
            
            string newtext = Parser.ExtractContent(text, Start, End, true, false);
            IHtmlTable table = HtmlTableParser.Parse(newtext + End);

            int setIndex = 0;
            int iconIndex = 1;
            int abrIndex = 2;

            ret = new List<EditionIconInfo>();
            for (int row = 0; row < table.RowCount; row++)
            {
                string set = null;
                string urlicon = null;
                string abr = null;

                int count = table.GetColCount(row);

                for (int col = 0; col < count; col++)
                {
                    IHtmlCell cell = table[row, col];

                    if (cell == null)
                        continue;

                    if (cell.IsHeader)
                    {
                        string innerText = cell.InnerText;
                        if (innerText.IndexOf("Set", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            setIndex = col;
                        else if (innerText.IndexOf("Sym.", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            iconIndex = col;
                        else if (innerText.IndexOf("Abrv.", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            abrIndex = col;
                    }
                    else
                    {
                        if (col == setIndex)
                            set = cell.InnerText.HtmlRemoveFormatTag();
                        else if (col == iconIndex)
                            urlicon = ExtractUrl(cell.InnerText);
                        else if (col == abrIndex)
                            abr = cell.InnerText.HtmlRemoveFormatTag();
                    }
                }

                if (!string.IsNullOrEmpty(set) && !string.IsNullOrEmpty(urlicon))
                    ret.Add(new EditionIconInfo(set, urlicon, abr));
            }

            _cache.Add(url, ret);
            return ret;
        }

        private string ExtractUrl(string text)
        {
            string tmp = Parser.ExtractContent(text, UrlStart, UrlEnd, false, false);
            Match m = _urlRegex.Match(tmp);
            return m.Success ? m.Groups["url"].Value : null;
        }
    }
}
