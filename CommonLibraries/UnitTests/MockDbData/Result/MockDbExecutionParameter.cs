namespace MockDbData
{
    using System.Data;

    public class MockDbExecutionParameter
    {
        internal MockDbExecutionParameter(IDbDataParameter parameter)
        {
            Precision = parameter.Precision;
            DbType = parameter.DbType;
            Scale = parameter.Scale;
            Size = parameter.Size;
            Direction = parameter.Direction;
            IsNullable = parameter.IsNullable;
            ParameterName = parameter.ParameterName;
            SourceColumn = parameter.SourceColumn;
            Value = parameter.Value;
        }

        public byte Precision { get; }
        public byte Scale { get; }
        public int Size { get; }
        public DbType DbType { get; }
        public ParameterDirection Direction { get; }
        public bool IsNullable { get; }
        public string ParameterName { get; }
        public string SourceColumn { get; }
        public object Value { get; }
    }
}