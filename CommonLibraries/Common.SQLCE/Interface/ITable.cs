namespace Common.SQLCE
{
    public interface ITable
    {
        string SchemaName { get; }
        string Name { get; }
        IColumn[] Columns();
        IIndex[] Indexes();
        IPrimaryKey PrimaryKey { get; }
        IColumn GetColumn(string name);
        IIndex GetIndex(string name);
        bool HasColumn(string name);
        bool HasIndex(string name);
    }
}