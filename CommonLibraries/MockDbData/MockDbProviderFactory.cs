namespace MockDbData
{
    using System;
    using System.Data.Common;

    public class MockDbProviderFactory : DbProviderFactory
    {
        private static readonly Lazy<MockDbProviderFactory> Lazy = new Lazy<MockDbProviderFactory>(() => new MockDbProviderFactory());

        public static MockDbProviderFactory Instance { get { return Lazy.Value; } }

        private MockDbProviderFactory()
        {
        }

        public override DbCommand CreateCommand()
        {
            return new MockDbCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new MockDbConnection();
        }

        public override DbParameter CreateParameter()
        {
            return new MockDbParameter();
        }
    }
}