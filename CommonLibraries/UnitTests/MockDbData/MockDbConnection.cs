namespace MockDbData
{
    using System.Data;
    using System.Data.Common;

    public class MockDbConnection : DbConnection, IAcceptResultInjection
    {
        private MockDbResultInjector _injector;
        private string _database;
        private ConnectionState _state = ConnectionState.Closed;

        public override string ConnectionString { get; set; }
        public override string Database { get { return _database; } }
        public override string DataSource { get; }
        public override string ServerVersion { get; }
        public override ConnectionState State { get { return _state; } }

        public override void ChangeDatabase(string databaseName)
        {
            _database = databaseName;
        }
        public override void Close()
        {
            _state = ConnectionState.Closed;
        }
        public override void Open()
        {
            _state = ConnectionState.Open;
        }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new MockDbTransaction(this, isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {

            MockDbCommand cmd = new MockDbCommand { Connection = this };
            ((IAcceptResultInjection)cmd).Accept(_injector);
            return cmd;
        }

        void IAcceptResultInjection.Accept(MockDbResultInjector injector)
        { 
            _injector = injector; 
        }

        protected override DbProviderFactory DbProviderFactory
        {
            get { return MockDbProviderFactory.Instance; }
        }
    }
}