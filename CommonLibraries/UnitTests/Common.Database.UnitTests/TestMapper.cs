namespace Common.Database.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using Common.Database;

    using MockDbData;

    using NUnit.Framework;

    [TestFixture]
    public class TestMapper
    {
        private MockDbConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connection = new MockDbConnection();
            _connection.Open();
        }

        [DbTable()]
        public class DbWithMultiColumn
        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
            [DbColumn()]
            public int Col3 { get; set; }
            [DbColumn()]
            public double Col4 { get; set; }
            [DbColumn()]
            public bool Col5 { get; set; }
            [DbColumn()]
            public object Col6 { get; set; }
            [DbColumn()]
            public ColumnKind Col7 { get; set; }
        }
        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Col1", typeof(string));
            dt.Columns.Add("Col2", typeof(string));
            dt.Columns.Add("Col3", typeof(int));
            dt.Columns.Add("Col4", typeof(double));
            dt.Columns.Add("Col5", typeof(bool));
            dt.Columns.Add("Col6", typeof(object));
            dt.Columns.Add("Col7", typeof(ColumnKind));

            return dt;
        }
        private void CheckMatchingResult(DbWithMultiColumn line, DataRow row)
        {
            Assert.That(line, Is.Not.Null);
            Assert.That(line.Col1, Is.EqualTo(row["Col1"]));
            Assert.That(line.Col2, Is.EqualTo(row["Col2"]));
            Assert.That(line.Col3, Is.EqualTo(row["Col3"]));
            Assert.That(line.Col4, Is.EqualTo(row["Col4"]));
            Assert.That(line.Col5, Is.EqualTo(row["Col5"]));
            Assert.That(line.Col6, Is.EqualTo(row["Col6"] == DBNull.Value ? null : row["Col6"]));
            Assert.That(line.Col7, Is.EqualTo((ColumnKind) row["Col7"]));
        }
        private void InjectGlobalResult(DataTable dataTable)
        {
            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddGlobalResult(new MockDbResult(dataTable));
            ((IAcceptResultInjection) _connection).Accept(injector);
        }

        [Test]
        public void TestLoadAll()
        {
            DataTable dt = CreateDataTable();
            dt.Rows.Add("aaaa", "Test", 42, 2.71, true, null, ColumnKind.Normal);
            dt.Rows.Add("bbbb", "Test2", 17, -3.14, false, 42, ColumnKind.PrimaryKey);
            InjectGlobalResult(dt);

            DbWithMultiColumn[] ret = Mapper<DbWithMultiColumn>.LoadAll(_connection).ToArray();
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret.Length, Is.EqualTo(dt.Rows.Count));

            for (int i = 0; i < ret.Length; i++)
            {
                CheckMatchingResult(ret[i], dt.Rows[i]);
            }
        }
        [Test]
        public void TestLoadOne()
        {
            DataTable dt = CreateDataTable();
            dt.Rows.Add("aaaa", "Test", 42, 2.71, true, null, ColumnKind.Normal);
            InjectGlobalResult(dt);

            DbWithMultiColumn source = new DbWithMultiColumn { Col1 = "aaaa" };
            DbWithMultiColumn ret = Mapper<DbWithMultiColumn>.Load(_connection, source);
            Assert.That(ret, Is.Not.Null);
            Assert.That(ret, Is.EqualTo(source));

            CheckMatchingResult(ret, dt.Rows[0]);
        }
        [Test]
        public void TestLoadOneNoData()
        {
            DataTable dt = CreateDataTable();
            InjectGlobalResult(dt);

            DbWithMultiColumn source = new DbWithMultiColumn { Col1 = "aaaa" };
            DbWithMultiColumn ret = Mapper<DbWithMultiColumn>.Load(_connection, source);
            Assert.That(ret, Is.Null);
        }

        [DbTable()]
        public class DbWithIdentity
        {
            [DbColumn(Kind = ColumnKind.Identity)]
            public int Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
        }
        [Test]
        public void TestInsertOneWithAddIdentifier()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Rows.Add(100);

            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddGlobalExecuteNonQueryResult(1);
            MockDbMatchingRule matchingRule = MockDbMatchingRule.CreateRule(CommandType.Text).SetParameterCount(0);
            injector.AddResult(matchingRule, new MockDbResult(dt));
            ((IAcceptResultInjection) _connection).Accept(injector);

            DbWithIdentity newClass = new DbWithIdentity { Col2 = "aaaa" };
            Mapper<DbWithIdentity>.InsertOne(_connection, newClass);

            Assert.That(newClass, Is.Not.Null);
            Assert.That(newClass.Col2, Is.EqualTo("aaaa"));
            Assert.That(newClass.Col1, Is.EqualTo(100));
        }
        [DbTable()]
        public class DbWithPrimary
        {
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public int Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
        }
        [TestCaseSource(nameof(TestInsertUpdateDeleteOneCases), new object[] { nameof(TestInsertUpdateDeleteOne) })]
        public void TestInsertUpdateDeleteOne(Action<IDbConnection, DbWithPrimary> func, int resultCount)
        {
            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddGlobalExecuteNonQueryResult(resultCount);
            ((IAcceptResultInjection) _connection).Accept(injector);

            DbWithPrimary newClass = new DbWithPrimary { Col1 = 10, Col2 = "aaaa" };

            if (resultCount == 1)
            {
                func(_connection, newClass);
                Assert.That(newClass, Is.Not.Null);
                Assert.That(newClass.Col2, Is.EqualTo("aaaa"));
                Assert.That(newClass.Col1, Is.EqualTo(10));
            }
            else
            {
                Assert.Throws(Is.TypeOf<ApplicationDbException>().With.Message.EqualTo("Wrong number of row affected. Rollback"), () => func(_connection, newClass));
            }
        }

        public static IEnumerable<TestCaseData> TestInsertUpdateDeleteOneCases(string methodCaller)
        {
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.InsertOne, 1).SetName($"{methodCaller} (InsertOne)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.InsertOne, 0).SetName($"{methodCaller} (InsertOneFailIfNoLineUpdated)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.InsertOne, 2).SetName($"{methodCaller} (InsertOneFailIfMultipleLineUpdated)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.DeleteOne, 1).SetName($"{methodCaller} (DeleteOne)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.DeleteOne, 0).SetName($"{methodCaller} (DeleteOneFailIfNoLineUpdated)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.DeleteOne, 2).SetName($"{methodCaller} (DeleteOneFailIfMultipleLineUpdated)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.UpdateOne, 1).SetName($"{methodCaller} (UpdateOne)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.UpdateOne, 0).SetName($"{methodCaller} (UpdateOneFailIfNoLineUpdated)");
            yield return new TestCaseData((Action<IDbConnection, DbWithPrimary>) Mapper<DbWithPrimary>.UpdateOne, 2).SetName($"{methodCaller} (UpdateOneFailIfMultipleLineUpdated)");
        }
    }
}