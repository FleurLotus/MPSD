namespace Common.Database.UnitTests
{
    using System;
    using System.Data;
    using System.Linq;

    using MockDbData;
    using Common.Database;

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
        private class DbWithMultiColumn
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
            Assert.That(line.Col7, Is.EqualTo((ColumnKind)row["Col7"]));
        }
        private void InjectGlobalResult(DataTable dataTable)
        {
            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddGlobalResult(new MockDbResult(dataTable));
            ((IAcceptResultInjection)_connection).Accept(injector);
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
    }
}