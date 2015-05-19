namespace Common.SQL
{
    public interface IIndex
    {
        string Name { get; }
        string TableName { get; }
        string SchemaName { get; }
        bool IsUnique { get; }
        bool? IsClustered { get; }
        IColumnForIndex[] Columns();
    }
}