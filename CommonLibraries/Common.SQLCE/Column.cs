namespace Common.SQLCE
{
    using System;
    using System.Globalization;

    internal class Column : IColumn, IComparable<IColumn>
    {
        public string Name { get; set; }
        public bool IsNullable { get; set; }
        public string DataType { get; set; }
        public int CharacterMaxLength { get; set; }
        public short NumericPrecision { get; set; }
        public short NumericScale { get; set; }
        public long AutoIncrementBy { get; set; }
        public long AutoIncrementSeed { get; set; }
        public long AutoIncrementNext { get; set; }
        public bool HasDefault { get; set; }
        public string Default { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public int Position { get; set; }
        public bool RowGuidCol { get; set; }
        public int Width { get; set; }
        public bool PadLeft { get; set; }
        public CaseSensitivity CaseSensitivity { get; internal set; }
        public string ShortType
        {
            get
            {
                if (DataType == "nchar" || DataType == "nvarchar" || DataType == "binary" || DataType == "varbinary")
                    return string.Format(CultureInfo.InvariantCulture, "{0}({1})", new object[] { DataType, CharacterMaxLength });

                if (DataType == "numeric")
                    return string.Format(CultureInfo.InvariantCulture, "{0}({1},{2})", new object[] { DataType, NumericPrecision, NumericScale });

                return string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { DataType });
            }
        }

        public int CompareTo(IColumn other)
        {
            int comp = 0;
            if (string.IsNullOrEmpty(SchemaName))
            {
                if (!string.IsNullOrEmpty(other.SchemaName))
                    comp = -1;
            }
            else
            {
                comp = string.Compare(SchemaName, other.SchemaName, StringComparison.Ordinal);
            }

            if (comp == 0)
                comp = string.Compare(TableName, other.TableName, StringComparison.Ordinal);

            if (comp == 0)
                comp = Position.CompareTo(other.Position);

            return comp;
        }
        public override string ToString()
        {
            return string.Format("{0}.{1}", Table.TableKey(SchemaName, TableName, CaseSensitivity), Name);
        }
    }
}