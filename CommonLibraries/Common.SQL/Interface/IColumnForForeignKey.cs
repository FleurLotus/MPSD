namespace Common.SQL
{
    public interface IColumnForForeignKey
    {
        IColumn SourceColumn { get; }
        IColumn ReferenceColumn { get; }
        int Position { get; }
    }
}