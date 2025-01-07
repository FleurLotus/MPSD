namespace Common.SQL
{
    public interface ITable
    {
        string SchemaName { get; }
        string Name { get; }
        IColumn[] Columns();
        IIndex[] Indexes();
        IForeignKey[] ForeignKeys();
        IPrimaryKey PrimaryKey { get; }
        IColumn GetColumn(string name);
        IIndex GetIndex(string name);
        IForeignKey GetForeignKey(string name);
        bool HasColumn(string name);
        bool HasIndex(string name);
    }
}