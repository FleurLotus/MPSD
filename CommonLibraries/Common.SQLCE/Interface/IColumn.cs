namespace Common.SQLCE
{
    public interface IColumn
    {
        string Name { get; }
        bool IsNullable { get; }
        string DataType { get; }
        int CharacterMaxLength { get; }
        short NumericPrecision { get; }
        short NumericScale { get; }
        long AutoIncrementBy { get; }
        long AutoIncrementSeed { get; }
        long AutoIncrementNext { get; }
        bool HasDefault { get; }
        string Default { get; }
        bool RowGuidCol { get; }
        int Width { get; }
        bool PadLeft { get; }
        string ShortType { get; }
        string TableName { get; }
        string SchemaName { get; }
        int Position { get; }
    }
}