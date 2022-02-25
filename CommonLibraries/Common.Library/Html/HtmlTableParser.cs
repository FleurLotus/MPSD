
namespace Common.Library.Html
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Common.Library.Exception;

    public static class HtmlTableParser
    {
        //TODO: less strict on closing tag use next tag to know if it close the previous one
        private const string TableStart = "<table";
        private const string TableEnd = "</table>";
        private const string RowStart = "<tr";
        private const string RowEnd = "</tr>";
        private const string RowCellStart = "<td";
        private const string RowCellEnd = "</td>";
        private const string RowCellHeaderStart = "<th";
        private const string RowCellHeaderEnd = "</th>";
        private const string Close = ">";
        private const string AutoEnd = "/>";
        private static readonly Regex _colSpanRegex = new Regex(@"colspan=""?(?<size>\d+)""?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex _rowSpanRegex = new Regex(@"rowspan=""?(?<size>\d+)""?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static IHtmlTable Parse(string htmlText)
        {
            string workingText = ExtractTable(htmlText);

            if (string.IsNullOrWhiteSpace(workingText))
            {
                return null;
            }

            return new HtmlTable(ExtractRows(workingText).Select(ExtractCells));
        }

        private static string ExtractTable(string htmlText)
        {
            int start = htmlText.IndexOf(TableStart, StringComparison.InvariantCultureIgnoreCase);
            //Can't find start tag
            if (start < 0)
            {
                return null;
            }

            string workingText = htmlText[start..];

            //Multi start tag
            if (workingText.IndexOf(TableStart, TableStart.Length, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                throw new HtmlTableParserMultiTableException();
            }

            int end = GetPostClosingIndex(workingText,0 ,TableEnd);
            if (end < 0)
            {
                throw new HtmlTableParserNoTableClosingTagException();
            }

            return workingText[..end];
        }
        private static IEnumerable<string> ExtractRows(string htmlText)
        {
            List<string> rows = new List<string>();

            int lastindex = 0;

            while ((lastindex = htmlText.IndexOf(RowStart, lastindex, StringComparison.InvariantCultureIgnoreCase)) >= 0)
            {
                int end = GetPostClosingIndex(htmlText, lastindex, RowEnd);
                if (end < 0)
                {
                    throw new HtmlTableParserNoRowClosingTagException();
                }

                rows.Add(htmlText[lastindex..end]);
                lastindex = end;
            }
            return rows;
        }
        private static IHtmlCell[] ExtractCells(string htmlRow)
        {
            List<IHtmlCell> cells = new List<IHtmlCell>();

            int lastindex = 0;

            while (true)
            {
                string endTag = null;
                int tdIndex = htmlRow.IndexOf(RowCellStart, lastindex, StringComparison.InvariantCultureIgnoreCase);
                if (tdIndex >= 0)
                {
                    endTag = RowCellEnd;
                    lastindex = tdIndex;
                }

                int thIndex = htmlRow.IndexOf(RowCellHeaderStart, lastindex, StringComparison.InvariantCultureIgnoreCase);
                if (thIndex >= 0)
                {
                    if (tdIndex < 0 || thIndex < tdIndex)
                    {
                        endTag = RowCellHeaderEnd;
                        lastindex = thIndex;
                    }
                }

                if (string.IsNullOrEmpty(endTag))
                {
                    break;
                }

                int end = GetPostClosingIndex(htmlRow, lastindex, endTag);
                if (end < 0)
                {
                    throw new HtmlTableParserNoCellClosingTagException();
                }

                cells.Add(ExtractCell(htmlRow[lastindex..end]));
                lastindex = end;
            }

            return cells.ToArray();
        }
        internal static IHtmlCell ExtractCell(string htmlCell)
        {
            bool isHeader = htmlCell.StartsWith(RowCellHeaderStart, StringComparison.InvariantCultureIgnoreCase);
            int tagIndex = htmlCell.IndexOf(Close,StringComparison.InvariantCultureIgnoreCase);
            //Should never happen
            if (tagIndex < 0)
            {
                throw new HtmlTableParserNoTagEndException();
            }

            string tag = htmlCell[..(tagIndex + Close.Length)];

            int colspan = 1;
            Match m = _colSpanRegex.Match(tag);
            if (m.Success)
            {
                colspan = int.Parse(m.Groups["size"].Value);
            }

            int rowspan = 1;
            m = _rowSpanRegex.Match(tag);
            if (m.Success)
            {
                rowspan = int.Parse(m.Groups["size"].Value);
            }

            if (htmlCell.EndsWith(AutoEnd, StringComparison.InvariantCultureIgnoreCase))
            {
                return new HtmlCell(string.Empty, isHeader, colspan, rowspan);
            }

            return new HtmlCell(htmlCell.Substring(tagIndex + 1, htmlCell.Length - (isHeader ? RowCellHeaderEnd.Length : RowCellEnd.Length) - tagIndex - 1), isHeader, colspan, rowspan);
        }
        internal static int GetPostClosingIndex(string workingText, int startIndex, string wantedCloseTag)
        {
            //Check auto close
            int autoClose = GetAutoCloseIndex(workingText, startIndex);
            if (autoClose >= 0)
            {
                return autoClose + AutoEnd.Length;
            }

            int end = workingText.IndexOf(wantedCloseTag, startIndex, StringComparison.InvariantCultureIgnoreCase);

            //No closing tag
            if (end < 0)
            {
                return -1;
            }

            return end + wantedCloseTag.Length;
        }
        internal static int GetAutoCloseIndex(string htmlText, int startIndex)
        {
            int autoClose = htmlText.IndexOf(AutoEnd, startIndex, StringComparison.InvariantCultureIgnoreCase);
            int close = htmlText.IndexOf(Close, startIndex, StringComparison.InvariantCultureIgnoreCase);
            //If close before auto close not the good autoclose tag
            return close < autoClose ? -1 : autoClose;
        }
    }
}
