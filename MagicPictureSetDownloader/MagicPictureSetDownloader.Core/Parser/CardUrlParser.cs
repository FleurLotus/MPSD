namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class CardUrlParser : IParser<string>
    {
        private const string Start = @"class=""checklist""";
        private const string End = @"</table>";

        private static readonly Regex _cardLineRegex = new Regex(@"<tr class=""cardItem"">(?<row>.*?)</tr>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _cardNameUrlRegex = new Regex(@"<a class=""nameLink"" href=""(?<url>[^""]+)""[^>]*>(?<name>.*?)</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _cardColumnRegex = new Regex(@"<td class=""(?<class>\w+)"">(?<col>.*?)</td>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public IEnumerable<string> Parse(string text)
        {
            string tabletext = Parser.ExtractContent(text, Start, End, true, false);

            int maxIndexNumber = -1;

            foreach (Match match in _cardLineRegex.Matches(tabletext))
            {
                string row = match.Groups["row"].Value;
                IDictionary<string, string> columnInfos = ExtractColumnInfos(row);
                
                string number = Get(columnInfos, "number");
                if (!string.IsNullOrWhiteSpace(number))
                {
                    int indexNumber = int.Parse(number);
                    maxIndexNumber = indexNumber > maxIndexNumber ? indexNumber : maxIndexNumber;
                }
                Match m = _cardNameUrlRegex.Match(Get(columnInfos, "name"));
                if (!m.Success)
                {
                    throw new ParserException($"Can't parse name for{row}");
                }

                yield return m.Groups["url"].Value;
            }

            NextPageChecker.CheckHasNextPage(text, false);
        }

        private IDictionary<string, string> ExtractColumnInfos(string row)
        {
            return _cardColumnRegex.Matches(row).Cast<Match>()
                .ToDictionary(m => m.Groups["class"].Value, m => m.Groups["col"].Value);
        }
        private TValue Get<TKey, TValue>(IDictionary<TKey, TValue> dic, TKey key)
        {
            if (!dic.TryGetValue(key, out TValue value))
            {
                throw new ParserException($"Missing info {value}");
            }

            return value;
        }
    }
}
