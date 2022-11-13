namespace MockDbData
{
    using System.Data;
    using System.Data.Common;

    internal class MockDbTransaction : DbTransaction
    {
        public override IsolationLevel IsolationLevel { get; }

        public MockDbTransaction(DbConnection connection) : this(connection, IsolationLevel.ReadCommitted)
        {
        }
        public MockDbTransaction(DbConnection connection, IsolationLevel isolationLevel)
        {
            DbConnection = connection;
            IsolationLevel = isolationLevel;
        }
        protected override DbConnection DbConnection { get; }

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }
    }
}