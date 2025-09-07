namespace Common.SQL.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using NUnit.Framework;

    using MockDbData;

    [TestFixture]
    public class RepositoryBaseTest
    {
        private RepositoryBaseTesting _repositoryBaseTesting;
        private MockDbConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connection = new MockDbConnection();
            _connection.Open();

            _repositoryBaseTesting = new RepositoryBaseTesting(_connection);
        }

        [Test]
        public void TestAllTables()
        {
            ITable[] tables = _repositoryBaseTesting.AllTables();

            Assert.That(tables, Is.Not.Null);
            Assert.That(tables.Length, Is.EqualTo(3));

            Assert.That(tables.Select(t => t.ToString()), Is.EquivalentTo(new[] { "SchemaName.TableName", "SchemaName.TableName2", "TableName3" }));
        }
        [Test]
        public void TestTablesExists()
        {
            Assert.That(_repositoryBaseTesting.TableExists("SchemaName", "TableName"), Is.True);
            Assert.That(_repositoryBaseTesting.TableExists("SchemaName2", "TableName"), Is.False);
            Assert.That(_repositoryBaseTesting.TableExists("TableName"), Is.False);
            Assert.That(_repositoryBaseTesting.TableExists("SchemaName", "TableName2"), Is.True);
            Assert.That(_repositoryBaseTesting.TableExists("SchemaName2", "TableName2"), Is.False);
            Assert.That(_repositoryBaseTesting.TableExists("TableName2"), Is.False);
            Assert.That(_repositoryBaseTesting.TableExists("SchemaName", "TableName3"), Is.False);
            Assert.That(_repositoryBaseTesting.TableExists("SchemaName2", "TableName3"), Is.False);
            Assert.That(_repositoryBaseTesting.TableExists("TableName3"), Is.True);
        }
        [Test]
        public void TestGetTable()
        {
            Assert.That(_repositoryBaseTesting.GetTable("SchemaName", "TableName"), Is.Not.Null);
            Assert.That(_repositoryBaseTesting.GetTable("SchemaName2", "TableName"), Is.Null);
            Assert.That(_repositoryBaseTesting.GetTable("TableName"), Is.Null);
            Assert.That(_repositoryBaseTesting.GetTable("SchemaName", "TableName2"), Is.Not.Null);
            Assert.That(_repositoryBaseTesting.GetTable("SchemaName2", "TableName2"), Is.Null);
            Assert.That(_repositoryBaseTesting.GetTable("TableName2"), Is.Null);
            Assert.That(_repositoryBaseTesting.GetTable("SchemaName", "TableName3"), Is.Null);
            Assert.That(_repositoryBaseTesting.GetTable("SchemaName2", "TableName3"), Is.Null);
            Assert.That(_repositoryBaseTesting.GetTable("TableName3"), Is.Not.Null);
        }
        [Test]
        public void TestColumnExists()
        {
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName", "TableName", "ColumnName"), Is.True);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName", "TableName", "ColumnName2"), Is.True);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName", "TableName", "ColumnName3"), Is.False);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName2", "TableName", "ColumnName"), Is.False);
            Assert.That(_repositoryBaseTesting.ColumnExists("TableName", "ColumnName"), Is.False);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName", "TableName2", "ColumnName"), Is.True);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName", "TableName2", "ColumnName2"), Is.True);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName", "TableName2", "ColumnName3"), Is.True);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName2", "TableName2", "ColumnName"), Is.False);
            Assert.That(_repositoryBaseTesting.ColumnExists("TableName2", "ColumnName"), Is.False);
            Assert.That(_repositoryBaseTesting.ColumnExists("SchemaName", "TableName3", "ColumnName"), Is.False);
            Assert.That(_repositoryBaseTesting.ColumnExists("TableName3", "ColumnName"), Is.True);
            Assert.That(_repositoryBaseTesting.ColumnExists("TableName3", "ColumnName2"), Is.False);
        }
        [TestCaseSource(nameof(TestToSqlStringEscapedSource), new object[] { nameof(TestToSqlStringEscaped) })]
        public string TestToSqlStringEscaped(string str)
        {
            return RepositoryBase.ToSqlStringEscaped(str);
        }
        public static IEnumerable<TestCaseData> TestToSqlStringEscapedSource(string methodCaller)
        {
            yield return new TestCaseData(null).Returns(null).SetName($"{methodCaller} (null)");
            yield return new TestCaseData(string.Empty).Returns(string.Empty).SetName($"{methodCaller} (Empty)");
            yield return new TestCaseData("azerty").Returns("azerty").SetName($"{methodCaller} (basic)");
            yield return new TestCaseData("'azc'q''sdf'").Returns("''azc''q''''sdf''").SetName($"{methodCaller} (with ')");
            yield return new TestCaseData("a\"\t\r\n, :/?\\").Returns("a\"\t\r\n, :/?\\").SetName($"{methodCaller} (with special characters)");
        }
        [Test]
        public void TestRowExistsUnknownTable()
        {
            Assert.Throws(Is.TypeOf<Exception>().With.Message.StartsWith("Unknown table"), () => _repositoryBaseTesting.RowExists("SchemaName2", "TableName", new[] { "ColumnName" }, new object[] { "0" }));
        }
        [Test]
        public void TestRowExistsNoColumns()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("columnNames"), () => _repositoryBaseTesting.RowExists("SchemaName", "TableName", null, new object[] { "0" }));
        }
        [Test]
        public void TestRowExistsEmptyColumns()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("columnNames"), () => _repositoryBaseTesting.RowExists("SchemaName", "TableName", Array.Empty<string>(), new object[] { "0" }));
        }
        [Test]
        public void TestRowExistsNoValues()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("values"), () => _repositoryBaseTesting.RowExists("SchemaName", "TableName", new[] { "ColumnName" }, null));
        }
        [Test]
        public void TestRowExistsEmptyValues()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("values"), () => _repositoryBaseTesting.RowExists("SchemaName", "TableName", new[] { "ColumnName" }, Array.Empty<object>()));
        }
        [Test]
        public void TestRowExistsUnmatchingListColumnsValues()
        {
            Assert.Throws(Is.TypeOf<Exception>().With.Message.StartsWith("columnNames and values must have the same length"), () => _repositoryBaseTesting.RowExists("SchemaName", "TableName", new[] { "ColumnName" }, new object[] { "0", 2 }));
        }
        [Test]
        public void TestRowExistsUnknownColumn()
        {
            Assert.Throws(Is.TypeOf<Exception>().With.Message.StartsWith("Unknown column"), () => _repositoryBaseTesting.RowExists("SchemaName", "TableName", new[] { "ColumnName4" }, new object[] { "0" }));
        }
        [Test]
        public void TestRowExists()
        {
            DataTable dt = new DataTable(); 
            dt.Columns.Add("Col1", typeof(string)); 
            dt.Rows.Add("aaaa");
            MockDbMatchingRule matchingRule = MockDbMatchingRule.CreateRule(CommandType.Text).SetCommandTextReg($"SELECT 1 FROM SchemaName.TableName WHERE \\(\\[ColumnName\\] = @ColumnName\\) AND \\(\\[ColumnName2\\] IS NULL\\)").SetParameterCount(1).SetParameterValue(0, "0");
            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddResult(matchingRule, new MockDbResult(dt));
            ((IAcceptResultInjection)_connection).Accept(injector);
            Assert.That(_repositoryBaseTesting.RowExists("SchemaName", "TableName", new[] { "ColumnName", "ColumnName2" }, new object[] { "0", null }), Is.True);
        }
        [Test]
        public void TestRowExistsNotFound()
        {
            Assert.That(_repositoryBaseTesting.RowExists("SchemaName", "TableName", new[] { "ColumnName", "ColumnName2" }, new object[] { "0", null }), Is.False);
        }

        [Test]
        public void TestExecuteBatch()
        {
            string sql = "UPDATE SchemaName.TableName SET ColumnName = 'a'";
            MockDbResultInjector injector = new MockDbResultInjector();
            ((IAcceptResultInjection)_connection).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));
            _repositoryBaseTesting.ExecuteBatch(sql);
            Assert.That(injector.Executions.Count, Is.EqualTo(1));
            MockDbExecution execution = injector.Executions[0];
            Assert.That(execution, Is.Not.Null);
            Assert.That(execution.Parameters.Count, Is.EqualTo(0));
            Assert.That(execution.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(execution.CommandText, Is.EqualTo(sql));
        }
        [Test]
        public void TestExecuteBatchWithFormat()
        {
            string sql = "UPDATE SchemaName.TableName SET ColumnName = '{0}' WHERE ColumnName = '{1}'";
            MockDbResultInjector injector = new MockDbResultInjector();
            ((IAcceptResultInjection)_connection).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));
            _repositoryBaseTesting.ExecuteBatch(sql, "aa'aa", "aaaa");
            Assert.That(injector.Executions.Count, Is.EqualTo(1));
            MockDbExecution execution = injector.Executions[0];
            Assert.That(execution, Is.Not.Null);
            Assert.That(execution.Parameters.Count, Is.EqualTo(0));
            Assert.That(execution.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(execution.CommandText, Is.EqualTo("UPDATE SchemaName.TableName SET ColumnName = 'aa''aa' WHERE ColumnName = 'aaaa'"));
        }
        [Test]
        public void TestExecuteParametrizeCommand()
        {
            string sql = "UPDATE SchemaName.TableName SET ColumnName = @Param1 WHERE ColumnName = @Param2";
            MockDbResultInjector injector = new MockDbResultInjector();
            ((IAcceptResultInjection)_connection).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));
            _repositoryBaseTesting.ExecuteParametrizeCommand(sql, new KeyValuePair<string, object>("@Param1", "aaa"), new KeyValuePair<string, object>("@Param2", "bbb")); 
            
            Assert.That(injector.Executions.Count, Is.EqualTo(1));
            MockDbExecution execution = injector.Executions[0];
            Assert.That(execution, Is.Not.Null);
            Assert.That(execution.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(execution.CommandText, Is.EqualTo(sql));
            Assert.That(execution.Parameters.Count, Is.EqualTo(2));
            MockDbExecutionParameter param1 = execution.Parameters[0];
            Assert.That(param1, Is.Not.Null);
            Assert.That(param1.ParameterName, Is.EqualTo("@Param1"));
            Assert.That(param1.Value, Is.EqualTo("aaa"));
            MockDbExecutionParameter param2 = execution.Parameters[1];
            Assert.That(param2, Is.Not.Null);
            Assert.That(param2.ParameterName, Is.EqualTo("@Param2"));
            Assert.That(param2.Value, Is.EqualTo("bbb"));
        }
        [Test]
        public void TestExecuteParametrizeCommandWithNullValue()
        {
            string sql = "UPDATE SchemaName.TableName SET ColumnName = @Param1 WHERE ColumnName = @Param2";
            MockDbResultInjector injector = new MockDbResultInjector();
            ((IAcceptResultInjection)_connection).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));
            _repositoryBaseTesting.ExecuteParametrizeCommand(sql, new KeyValuePair<string, object>("@Param1", "aaa"), new KeyValuePair<string, object>("@Param2", null));

            Assert.That(injector.Executions.Count, Is.EqualTo(1));
            MockDbExecution execution = injector.Executions[0];
            Assert.That(execution, Is.Not.Null);
            Assert.That(execution.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(execution.CommandText, Is.EqualTo(sql));
            Assert.That(execution.Parameters.Count, Is.EqualTo(2));
            MockDbExecutionParameter param1 = execution.Parameters[0];
            Assert.That(param1, Is.Not.Null);
            Assert.That(param1.ParameterName, Is.EqualTo("@Param1"));
            Assert.That(param1.Value, Is.EqualTo("aaa"));
            MockDbExecutionParameter param2 = execution.Parameters[1];
            Assert.That(param2, Is.Not.Null);
            Assert.That(param2.ParameterName, Is.EqualTo("@Param2"));
            Assert.That(param2.Value, Is.EqualTo(DBNull.Value));
        }
        [Test]
        public void TestExecuteParametrizeCommandMulti()
        {
            string sql = "UPDATE SchemaName.TableName SET ColumnName = @Param1 WHERE ColumnName = @Param2";
            MockDbResultInjector injector = new MockDbResultInjector();
            ((IAcceptResultInjection)_connection).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));

            var param = new List<KeyValuePair<string, object>[]>
            {
                    new[] { new KeyValuePair<string, object>("@Param1", "aaa"), new KeyValuePair<string, object>("@Param2", "bbb") },
                    new[] { new KeyValuePair<string, object>("@Param1", "111"), new KeyValuePair<string, object>("@Param2", null) },
            };

            _repositoryBaseTesting.ExecuteParametrizeCommandMulti(sql, param);

            Assert.That(injector.Executions.Count, Is.EqualTo(2));
            MockDbExecution execution = injector.Executions[0];
            Assert.That(execution, Is.Not.Null);
            Assert.That(execution.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(execution.CommandText, Is.EqualTo(sql));
            
            Assert.That(execution.Parameters.Count, Is.EqualTo(2));
            MockDbExecutionParameter param1 = execution.Parameters[0];
            Assert.That(param1, Is.Not.Null);
            Assert.That(param1.ParameterName, Is.EqualTo("@Param1"));
            Assert.That(param1.Value, Is.EqualTo("aaa"));
            
            MockDbExecutionParameter param2 = execution.Parameters[1];
            Assert.That(param2, Is.Not.Null);
            Assert.That(param2.ParameterName, Is.EqualTo("@Param2"));
            Assert.That(param2.Value, Is.EqualTo("bbb"));

            execution = injector.Executions[1];
            Assert.That(execution, Is.Not.Null);
            Assert.That(execution.CommandType, Is.EqualTo(CommandType.Text));
            Assert.That(execution.CommandText, Is.EqualTo(sql));

            Assert.That(execution.Parameters.Count, Is.EqualTo(2));
            param1 = execution.Parameters[0];
            Assert.That(param1, Is.Not.Null);
            Assert.That(param1.ParameterName, Is.EqualTo("@Param1"));
            Assert.That(param1.Value, Is.EqualTo("111"));
            param2 = execution.Parameters[1];
            Assert.That(param2, Is.Not.Null);
            Assert.That(param2.ParameterName, Is.EqualTo("@Param2"));
            Assert.That(param2.Value, Is.EqualTo(DBNull.Value));
        }
        [Test]
        public void TestExecuteParametrizeCommandMultiWithEmptyCommand()
        {
            string sql = "\r\n";
            MockDbResultInjector injector = new MockDbResultInjector();
            ((IAcceptResultInjection)_connection).Accept(injector);
            Assert.That(injector.Executions.Count, Is.EqualTo(0));

            var param = new List<KeyValuePair<string, object>[]>
            {
                    new[] { new KeyValuePair<string, object>("@Param1", "aaa"), new KeyValuePair<string, object>("@Param2", "bbb") },
                    new[] { new KeyValuePair<string, object>("@Param1", "111"), new KeyValuePair<string, object>("@Param2", null) },
            };

            _repositoryBaseTesting.ExecuteParametrizeCommandMulti(sql, param);

            Assert.That(injector.Executions.Count, Is.EqualTo(0));
        }
    }
}
