namespace MockDbData.UnitTests
{
    using System.Data;

    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbParameter
    {
        [Test]
        public void TestConstructorByValue()
        {
            MockDbParameter parameter = new MockDbParameter("azerty", 42);
            Assert.That(parameter.Value, Is.EqualTo(42));
            Assert.That(parameter.ParameterName, Is.EqualTo("azerty"));
        }
        [Test]
        public void TestConstructorByType()
        {
            MockDbParameter parameter = new MockDbParameter("azerty", DbType.Boolean);
            Assert.That(parameter.DbType, Is.EqualTo(DbType.Boolean));
            Assert.That(parameter.ParameterName, Is.EqualTo("azerty"));
        }
        [Test]
        public void TestProperties()
        {
            MockDbParameter parameter = new MockDbParameter
            {
                DbType = DbType.String
            };
            Assert.That(parameter.DbType, Is.EqualTo(DbType.String));

            parameter.Value = 63.5;
            Assert.That(parameter.Value, Is.EqualTo(63.5));

            parameter.Direction = ParameterDirection.InputOutput;
            Assert.That(parameter.Direction, Is.EqualTo(ParameterDirection.InputOutput));

            parameter.ParameterName = "azerty";
            Assert.That(parameter.ParameterName, Is.EqualTo("azerty"));

            parameter.IsNullable = true;
            Assert.That(parameter.IsNullable, Is.True);

            parameter.Size = 10;
            Assert.That(parameter.Size, Is.EqualTo(10));

            parameter.SourceColumn = "qsdfg";
            Assert.That(parameter.SourceColumn, Is.EqualTo("qsdfg"));

            parameter.SourceColumnNullMapping = true;
            Assert.That(parameter.SourceColumnNullMapping, Is.True);

        }
    }
}