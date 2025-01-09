namespace MockDbData.UnitTests
{
    using System.Data;
    using System.Data.Common;

    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbConnection
    {
        [Test]
        public void TestProperties()
        {
            MockDbConnection connection = new MockDbConnection();

            connection.ConnectionString = "azerty";
            Assert.That(connection.ConnectionString, Is.EqualTo("azerty"));
        }
        [Test]
        public void TestCreateCommand()
        {
            MockDbConnection connection = new MockDbConnection();
            DbCommand command = connection.CreateCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command, Is.InstanceOf(typeof(MockDbCommand)));
        }
        [Test]
        public void TestBeginTransaction()
        {
            MockDbConnection connection = new MockDbConnection();
            DbTransaction transaction = connection.BeginTransaction(IsolationLevel.Chaos);
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction, Is.InstanceOf(typeof(MockDbTransaction)));
            Assert.That(transaction.IsolationLevel, Is.EqualTo(IsolationLevel.Chaos));
            Assert.That(transaction.Connection, Is.EqualTo(connection));
        }
        [Test]
        public void TestGetFactory()
        {
            MockDbConnection connection = new MockDbConnection();
            DbProviderFactory factory = DbProviderFactories.GetFactory(connection);
            Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.InstanceOf(typeof(MockDbProviderFactory)));
        }
        [Test]
        public void TestChangeDatabase()
        {
            MockDbConnection connection = new MockDbConnection();
            Assert.That(connection.Database, Is.Null);
            connection.ChangeDatabase("azerty");
            Assert.That(connection.Database, Is.EqualTo("azerty"));
        }
        [Test]
        public void TestState()
        {
            MockDbConnection connection = new MockDbConnection();
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
            connection.Open();
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            connection.Close();
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
        }
    }
}