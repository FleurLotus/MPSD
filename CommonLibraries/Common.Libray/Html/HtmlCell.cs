namespace Common.Libray.Html
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
        
        public bool IsHeader { get; private set; }
        public string InnerText { get; private set; }
        public int ColSpan { get; private set; }
        public int RowSpan { get; private set; }
        public override string ToString()
        {
            return InnerText;
        }
    }
}
