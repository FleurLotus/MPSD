namespace MockDbData
{
    using System.Data;
    using System.Data.Common;

    public class MockDbConnection : DbConnection, IAcceptResultInjection
    {
        private MockDbResultInjector _injector;

        public override string ConnectionString { get; set; }
        public override string Database { get; }
        public override string DataSource { get; }
        public override string ServerVersion { get; }
        public override ConnectionState State { get; }

        public override void ChangeDatabase(string databaseName)
        {
        }
        public override void Close()
        {
        }
        public override void Open()
        {
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