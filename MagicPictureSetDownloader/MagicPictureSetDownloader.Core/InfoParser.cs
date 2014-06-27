using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace MagicPictureSetDownloader.Core
{
    internal class InfoParser
    {
        private const string SelectSetStart = @"<select name=""ctl00$ctl00$MainContent$Content$SearchControls$setAddText""";
        private const string SelectSetEnd = "</select>";
        private const string SetCheckListStart = @"class=""checklist""";
        private const string SetCheckListEnd = @"</table>";

        private readonly Regex _setRegex = new Regex(@"<option value=""(?<setname>[^""]+)"">.*?</option>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly Regex _searchPageRegex = new Regex(@"var cardSearchPage = '(?<url>[^']+)';", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _cardLineRegex = new Regex(@"<tr class=""cardItem"">(?<row>.*?)</tr>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _cardColumnRegex = new Regex(@"<td class=""(?<class>\w+)"">(?<col>.*?)</td>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _cardNameUrlRegex = new Regex(@"<a class=""nameLink"" href=""(?<url>[^""]+)""[^>]*>(?<name>.*?)</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal IEnumerable<SetInfo> ParseSetsList(string htmltext)
        {
            Match match = _searchPageRegex.Match(htmltext);
            if (!match.Success)
                throw new ParserException("Can't find seach page");

            string url = match.Groups["url"].Value;

            string content = ExtractContent(WebUtility.HtmlDecode(htmltext), SelectSetStart, SelectSetEnd);

            return _setRegex.Matches(content).Cast<Match>()
                                              .Select(m => new SetInfo(m.Groups["setname"].Value, url));
        }

        internal IEnumerable<CardInfo> ParseCardInfos(string htmltext)
        {
            IList<CardInfo> cardInfos = new List<CardInfo>();
            string content = ExtractContent(WebUtility.HtmlDecode(htmltext), SetCheckListStart, SetCheckListEnd);
            int maxIndexNumber = -1;

            foreach (Match match in _cardLineRegex.Matches(content))
            {
                string row = match.Groups["row"].Value;
                IDictionary<string, string> columnInfos = ExtractColumnInfos(row);
                
                
                string number = Get(columnInfos, "number");
                int indexNumber = int.Parse(number);
                maxIndexNumber = indexNumber > maxIndexNumber ? indexNumber : maxIndexNumber;
                
                string rarity = Get(columnInfos, "rarity");

                Match m = _cardNameUrlRegex.Match(Get(columnInfos, "name"));
                if (!m.Success)
                    throw new ParserException("Can't parse name for" + row);

                cardInfos.Add(new CardInfo(m.Groups["name"].Value, m.Groups["url"].Value, rarity));
            }

            if (maxIndexNumber != cardInfos.Count)
                throw new ParserException("Error while parsing, number of card info doesn't match max card id for set");

            return cardInfos;
        }

        private IDictionary<string, string> ExtractColumnInfos(string row)
        {
            return _cardColumnRegex.Matches(row).Cast<Match>()
                                                .ToDictionary(m => m.Groups["class"].Value, m => m.Groups["col"].Value);
        }
        private string ExtractContent(string htmltext, string start, string end)
        {
            int startIndex = htmltext.LastIndexOf(start, StringComparison.Ordinal);
            if (startIndex < 0)
                return htmltext;
            
            startIndex += start.Length;

            int endIndex = htmltext.IndexOf(end, startIndex, StringComparison.Ordinal);
            if (endIndex < 0)
                return htmltext;

            return htmltext.Substring(startIndex, endIndex - startIndex);
        }
        private TValue Get<TKey, TValue>(IDictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            if (!dic.TryGetValue(key, out value))
                throw new ParserException("Missing info " + value);

            return value;
        }

    }
}
