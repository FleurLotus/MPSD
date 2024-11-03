namespace MockDbData.UnitTests
{
    using System.Data.Common;
    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbProviderFactory
    {
        [Test]
        public void TestCreateCommand()
        {
            DbCommand command = MockDbProviderFactory.Instance.CreateCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command, Is.InstanceOf(typeof(MockDbCommand)));
        }
        [Test]
        public void TestCreateConnection()
        {
            DbConnection connection = MockDbProviderFactory.Instance.CreateConnection();
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection, Is.InstanceOf(typeof(MockDbConnection)));
        }
        [Test]
        public void TestCreateParameter()
        {
            DbParameter parameter = MockDbProviderFactory.Instance.CreateParameter();
            Assert.That(parameter, Is.Not.Null);
            Assert.That(parameter, Is.InstanceOf(typeof(MockDbParameter)));
        }
        [Test]
        public void TestOtherProperties()
        {
            Assert.That(MockDbProviderFactory.Instance.CanCreateDataAdapter, Is.False);
            Assert.That(MockDbProviderFactory.Instance.CanCreateBatch, Is.False);
            Assert.That(MockDbProviderFactory.Instance.CanCreateCommandBuilder, Is.False);
            Assert.That(MockDbProviderFactory.Instance.CanCreateDataSourceEnumerator, Is.False);

            Assert.That(MockDbProviderFactory.Instance.CreateConnectionStringBuilder, Is.Null);
            Assert.That(MockDbProviderFactory.Instance.CreateCommandBuilder, Is.Null);
        }
    }
}