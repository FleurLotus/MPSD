namespace Common.Library.Html
{
    internal class HtmlCell : IHtmlCell
    {
        public HtmlCell(string innerText, bool isHeader, int colSpan, int rowSpan)
        {
            RowSpan = rowSpan;
            ColSpan = colSpan;
            IsHeader = isHeader;
            InnerText = innerText;
        }
        
        public bool IsHeader { get; }
        public string InnerText { get; }
        public int ColSpan { get; }
        public int RowSpan { get; }
        public override string ToString()
        {
            return InnerText;
        }
    }
}
