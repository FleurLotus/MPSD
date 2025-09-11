namespace MockDbData.UnitTests
{
    using System.Data;
    using System.Data.Common;

    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbCommand
    {
        [Test]
        public void TestProperties()
        {
            MockDbCommand command = new MockDbCommand
            {
                CommandText = "azerty"
            };
            Assert.That(command.CommandText, Is.EqualTo("azerty"));

            command.CommandTimeout = 100;
            Assert.That(command.CommandTimeout, Is.EqualTo(100));

            command.CommandType = CommandType.StoredProcedure;
            Assert.That(command.CommandType, Is.EqualTo(CommandType.StoredProcedure));

            command.DesignTimeVisible = true;
            Assert.That(command.DesignTimeVisible, Is.True);

            command.UpdatedRowSource = UpdateRowSource.OutputParameters;
            Assert.That(command.UpdatedRowSource, Is.EqualTo(UpdateRowSource.OutputParameters));

            Assert.That(command.Parameters, Is.Not.Null);
        }
        [Test]
        public void TestExecuteReader()
        {
            MockDbCommand command = new MockDbCommand();
            DbDataReader reader = command.ExecuteReader();

            Assert.That(reader, Is.Not.Null);
            Assert.That(reader, Is.InstanceOf<MockDbDataReader>());
            Assert.That(reader.Read, Is.False);
        }

        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Col1", typeof(string));
            dt.Columns.Add("Col2", typeof(string));
            dt.Columns.Add("Col3", typeof(int));
            dt.Columns.Add("Col4", typeof(double));
            dt.Columns.Add("Col5", typeof(bool));
            return dt;
        }
        private void InjectGlobalResult(MockDbCommand command, DataTable dataTable)
        {
            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddGlobalResult(new MockDbResult(dataTable));
            ((IAcceptResultInjection) command).Accept(injector);
        }

        [Test]
        public void TestExecuteReaderWithInjection()
        {
            DataTable dt = CreateDataTable();
            dt.Rows.Add("aaaa", "Test", 42, 2.71, true);
            dt.Rows.Add("bbbb", "Test2", 17, -3.14, false);

            MockDbCommand command = new MockDbCommand();
            InjectGlobalResult(command, dt);

            DbDataReader reader = command.ExecuteReader();
            Assert.That(reader, Is.Not.Null);
            Assert.That(reader, Is.InstanceOf<MockDbDataReader>());
            Assert.That(reader.Read, Is.True);
        }
        [Test]
        public void TestExecuteScalar()
        {
            MockDbCommand command = new MockDbCommand();
            object ret = command.ExecuteScalar();

            Assert.That(ret, Is.Null);
        }
        [Test]
        public void TestExecuteScalarWithInjection()
        {
            DataTable dt = CreateDataTable();
            dt.Rows.Add("aaaa", "Test", 42, 2.71, true);
            dt.Rows.Add("bbbb", "Test2", 17, -3.14, false);

            MockDbCommand command = new MockDbCommand();
            InjectGlobalResult(command, dt);

            object ret = command.ExecuteScalar();
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret, Is.EqualTo(dt.Rows[0][0]));
        }
        [Test]
        public void TestExecuteScalarWithInjectionEmpty()
        {
            DataTable dt = new DataTable();
            MockDbCommand command = new MockDbCommand();
            InjectGlobalResult(command, dt);

            object ret = command.ExecuteScalar();
            Assert.That(ret, Is.Null);
        }
        [Test]
        public void TestExecuteNonQuery()
        {
            MockDbCommand command = new MockDbCommand();
            int result = command.ExecuteNonQuery();

            Assert.That(result, Is.EqualTo(0));
        }
        [Test]
        public void TestExecuteNonQueryWithInjection()
        {
            MockDbResultInjector injector = new MockDbResultInjector();

            MockDbCommand command = new MockDbCommand();
            ((IAcceptResultInjection) command).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));
            int result = command.ExecuteNonQuery();
            Assert.That(result, Is.EqualTo(0));
            Assert.That(injector.Executions.Count, Is.EqualTo(1));
        }
        [Test]
        public void TestExecuteNonQueryWithInjectionAndValue()
        {
            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddGlobalExecuteNonQueryResult(123);

            MockDbCommand command = new MockDbCommand();
            ((IAcceptResultInjection) command).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));

            int result = command.ExecuteNonQuery();
            Assert.That(result, Is.EqualTo(123));
            Assert.That(injector.Executions.Count, Is.EqualTo(1));
        }
        [Test]
        public void TestPrepare()
        {
            MockDbCommand command = new MockDbCommand();
            Assert.DoesNotThrow(() => command.Prepare());
        }
        [Test]
        public void TestCancel()
        {
            MockDbCommand command = new MockDbCommand();
            Assert.DoesNotThrow(() => command.Cancel());
        }
    }
}