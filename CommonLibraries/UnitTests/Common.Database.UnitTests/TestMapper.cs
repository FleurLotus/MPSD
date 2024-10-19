namespace Common.Database.UnitTests
{
    using System.Data;

    using MockDbData;
    using Common.Database;

    using NUnit.Framework;

    [TestFixture]
    public class TestMapper
    {
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
        }

        [Test]
        public void TestLoadAll()
        {
            MockDbConnection cnx = new MockDbConnection();
            cnx.Open();

            DataTable dt = new DataTable();
            dt.Columns.Add("Col1",typeof(string));
            dt.Columns.Add("Col2",typeof(string));
            dt.Columns.Add("Col3",typeof(int));
            dt.Columns.Add("Col4",typeof(double));
            dt.Columns.Add("Col5",typeof(bool));

            dt.Rows.Add("aaaa", "Test", 42, 2.71, true);
            dt.Rows.Add("bbbb", "Test2", 17, -3.14, false);
            MockDbResultInjector injector = new MockDbResultInjector();
            injector.AddGlobalResult(new MockDbResult(dt));
            ((IAcceptResultInjection)cnx).Accept(injector);
            var ret = Mapper<DbWithMultiColumn>.LoadAll(cnx);
        }
    }
}