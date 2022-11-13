namespace MockDbData
{
    using System;
    using System.Data;
    using System.Data.Common;

    public class MockDbCommand : DbCommand, IAcceptResultInjection
    {
        private MockDbResultInjector _injector;
        private readonly MockDbParameterCollection _parameters = new MockDbParameterCollection();

        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        public new MockDbParameterCollection Parameters { get { return _parameters; } }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _parameters; }
        }

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public override void Prepare()
        {
            throw new NotImplementedException();
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