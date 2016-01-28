namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class CardLanguageParser : IParser<CardLanguageInfo>
    {
        private const string Start = @"<table class=""cardList"" cellspacing=""0"" cellpadding=""2"">";
        private const string End = @"</table>";

        private static readonly Regex _languageRegex = new Regex(@">\s*(?<name>.*?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _cardNameRegex = new Regex(@"<a id="".*"" href="".*"">(?<name>.*)</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public IEnumerable<CardLanguageInfo> Parse(string text)
        {
            string newtext = Parser.ExtractContent(text, Start, End, true, false);

            bool header = true;
            int translateNameIndex = -1;
            int languageNameIndex = -1;

            foreach (string row in GetTableRow(newtext))
            {
                string trimedrow = row.Replace("\r", "").Replace("\n", "");
                if (trimedrow.Contains(@"<tr id=""ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_languageList_noOtherLanguagesParent"">"))
                    continue;

                if (trimedrow.Contains(@"<tr id=""ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_languageList_pagingControlsParent"">"))
                {
                    NextPageChecker.CheckHasNextPage(trimedrow, true);
                }

                string[] columns = trimedrow.Split(new[] { "</td>" }, StringSplitOptions.None);
                if (header)
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        string column = columns[i];
                        if (column.Contains(">Translated Card Name<"))
                            translateNameIndex = i;

                        if (column.Contains(">Language<"))
                            languageNameIndex = i;
                    }

                    if (languageNameIndex == -1 || translateNameIndex == -1)
                        throw new ParserException("Can't parse language page.");

                    header = false;
                    continue;
                }

                Match m = _cardNameRegex.Match(columns[translateNameIndex]);
                if (!m.Success)
                    throw new ParserException("Can't find card name in foreign language");

                string cardName = m.Groups["name"].Value.Trim();
                if (string.IsNullOrWhiteSpace(cardName))
                    throw new ParserException("Card name in foreign language is null or empty");

                m = _languageRegex.Match(columns[languageNameIndex]);
                if (!m.Success)
                    throw new ParserException("Can't find foreign language name");

                string languageName = m.Groups["name"].Value;
                if (string.IsNullOrWhiteSpace(languageName))
                    throw new ParserException("Foreign language name is null or empty");

                yield return new CardLanguageInfo { Language = languageName, Name = cardName };
            }
        }
        private IEnumerable<string> GetTableRow(string newtext)
        {
            int pos = 0;
            while (pos >= 0)
            {
                pos = newtext.IndexOf("<tr", pos, StringComparison.OrdinalIgnoreCase);
                if (pos == -1)
                    yield break;

                int end = newtext.IndexOf("</tr", pos, StringComparison.OrdinalIgnoreCase);
                if (end == -1 || end <= pos)
                    yield break;

                yield return newtext.Substring(pos, end - pos);

                pos = end;
            }
        }
    }
}
