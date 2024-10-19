namespace MockDbData
{
    using System.Data;
    using System.Data.Common;

    public class MockDbParameter : DbParameter
    {
        public MockDbParameter()
        {
        }

        public MockDbParameter(string parameterName, DbType parameterType)
        {
            ParameterName = parameterName;
            DbType = parameterType;
        }

        public MockDbParameter(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override int Size { get; set; }
        public override string SourceColumn { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override object Value { get; set; }

        public override void ResetDbType()
        {
        }
    }
}