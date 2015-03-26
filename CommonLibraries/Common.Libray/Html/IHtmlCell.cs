namespace Common.Libray.Html
{
    public interface IHtmlCell
    {
        bool IsHeader { get; }
        string InnerText { get; }
        int ColSpan { get; }
        int RowSpan { get; }
    }
}