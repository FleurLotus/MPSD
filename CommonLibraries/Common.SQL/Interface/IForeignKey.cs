namespace Common.SQL
{
    public interface IForeignKey
    {
        string Name { get; }

        string SourceTableName { get; }
        string SourceSchemaName { get; }

        string ReferenceName { get; }
        string ReferenceTableName { get; }
        string ReferenceSchemaName { get; }

        string UpdateRule { get; }
        string DeleteRule { get; }
        
        IColumnForForeignKey[] SourceColumns();
    }
}