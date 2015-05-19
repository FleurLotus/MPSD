namespace Common.SQL
{
    public interface IColumnForIndex
    {
        IColumn Column { get; }
        bool? IsAsc { get; }
        int Position { get; }
    }
}