namespace MockDbData.UnitTests
{
    using System.Data;

    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbExecutionParameter
    {
        [Test]
        public void TestConstructor()
        {
            MockDbParameter param = new MockDbParameter
            {
                DbType = DbType.String,
                Value = 63.5,
                Direction = ParameterDirection.InputOutput,
                ParameterName = "azerty",
                IsNullable = true,
                Size = 10,
                SourceColumn = "qsdfg",
                SourceColumnNullMapping = true
            };

            MockDbExecutionParameter parameter = new MockDbExecutionParameter(param);

            Assert.That(parameter.DbType, Is.EqualTo(DbType.String));
            Assert.That(parameter.Value, Is.EqualTo(63.5));
            Assert.That(parameter.Direction, Is.EqualTo(ParameterDirection.InputOutput));
            Assert.That(parameter.ParameterName, Is.EqualTo("azerty"));
            Assert.That(parameter.IsNullable, Is.True);
            Assert.That(parameter.Size, Is.EqualTo(10));
            Assert.That(parameter.SourceColumn, Is.EqualTo("qsdfg"));
            Assert.That(parameter.Precision, Is.EqualTo(0));
            Assert.That(parameter.Scale, Is.EqualTo(0));
        }
    }
}
