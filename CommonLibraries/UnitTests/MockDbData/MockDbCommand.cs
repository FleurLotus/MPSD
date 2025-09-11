namespace MockDbData
{
    using System.Data;
    using System.Data.Common;

    public class MockDbCommand : DbCommand, IAcceptResultInjection
    {
        private MockDbResultInjector _injector;

        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        public new MockDbParameterCollection Parameters { get; } = new MockDbParameterCollection();

        protected override DbParameterCollection DbParameterCollection
        {
            get { return Parameters; }
        }

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            return _injector?.GetExecuteNonQueryResult(this) ?? 0;
        }
        public override object ExecuteScalar()
        {
            MockDbResult result = _injector?.GetMockDbResult(this);
            if (result != null)
            {
                MockDbDataReader reader = new MockDbDataReader(result);
                if (reader.Read())
                {
                    return reader.GetValue(0);
                }
            }

            return null;
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new MockDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            MockDbResult result = _injector?.GetMockDbResult(this);
            if (result != null)
            {
                return new MockDbDataReader(result);
            }

            return new MockDbDataReader();
        }

        void IAcceptResultInjection.Accept(MockDbResultInjector injector)
        {
            _injector = injector;
        }
    }
}