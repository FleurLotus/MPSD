namespace MockDbData.UnitTests
{
    using System.Data;

    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbExecution
    {
        [Test]
        public void TestConstructor()
        {
            MockDbCommand cmd = new MockDbCommand
            {
                CommandText = "TEST",
                CommandType = CommandType.Text,
            };

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

            cmd.Parameters.Add(param);

            MockDbExecution command = new MockDbExecution(cmd);
            Assert.That(command.CommandText, Is.EqualTo("TEST"));
            Assert.That(command.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(command.Parameters, Is.Not.Null);
            Assert.That(command.Parameters.Count, Is.EqualTo(1));

            MockDbExecutionParameter parameter = command.Parameters[0];

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
