namespace Common.SQLCE
{
    public interface ITable
    {
        string SchemaName { get; }
        string Name { get; }
        IColumn[] Columns();
        IColumn GetColumn(string name);
        bool HasColumn(string name);
    }
}