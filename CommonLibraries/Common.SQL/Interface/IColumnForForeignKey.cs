namespace Common.SQL
{
    public interface IColumnForForeignKey
    {
        IColumn SourceColumn { get; }
        int SourcePosition { get; }

        IColumn ReferenceColumn { get; }
        int ReferencePosition { get; }
    }
}