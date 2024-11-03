namespace MockDbData.UnitTests
{
    using System.Data;
    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbTransaction
    {
        [Test]
        public void TestConstructor()
        {
            MockDbConnection connection = new MockDbConnection();
            MockDbTransaction transaction = new MockDbTransaction(connection);

            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Connection, Is.EqualTo(connection));
            Assert.That(transaction.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
        }
        [Test]
        public void TestConstructor2Parameters()
        {
            MockDbConnection connection = new MockDbConnection();
            MockDbTransaction transaction = new MockDbTransaction(connection, IsolationLevel.ReadUncommitted);

            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Connection, Is.EqualTo(connection));
            Assert.That(transaction.IsolationLevel, Is.EqualTo(IsolationLevel.ReadUncommitted));
        }
        [Test]
        public void TestCommit()
        {
            MockDbConnection connection = new MockDbConnection();
            MockDbTransaction transaction = new MockDbTransaction(connection);

            Assert.DoesNotThrow(() => transaction.Commit());
        }
        [Test]
        public void TestRollback()
        {
            MockDbConnection connection = new MockDbConnection();
            MockDbTransaction transaction = new MockDbTransaction(connection);

            Assert.DoesNotThrow(() => transaction.Rollback());
        }
    }
}