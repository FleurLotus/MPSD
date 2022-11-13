namespace Common.UnitTests.Database
{
    using System;
    using Common.Database;

    using NUnit.Framework;

    [TestFixture]
    public class TestDbAttributAnalyser
    {
        private class DbTableNotPresentClass
        {
        }

        [Test]
        public void TestDbTableIsPresent()
        {
            AttributedTypeException ex = Assert.Throws<AttributedTypeException>(() => DbAttributAnalyser.Analyse(typeof(DbTableNotPresentClass)));
            Assert.That(ex.Message, Is.EqualTo("DbTableAttribute must be declared one and one time for the type"));
        }

        [DbTable]
        private class DbColumnNotPresentClass
        {
            public string Col { get; set; }
        }

        [Test]
        public void TestDbColumnIsPresent()
        {
            AttributedTypeException ex = Assert.Throws<AttributedTypeException>(() => DbAttributAnalyser.Analyse(typeof(DbColumnNotPresentClass)));
            Assert.That(ex.Message, Is.EqualTo("DbColumnAttribute must be declared at least one time for the type"));
        }

        [DbTable]
        private class DbColumnIdentityNotUniqueClass
        {
            [DbColumn (Kind = ColumnKind.Identity)]
            public string Col1 { get; set; }
            [DbColumn (Kind = ColumnKind.Identity)]
            public string Col2 { get; set; }
        }

        [Test]
        public void TestDbColumnIdentityUnique()
        {
            AttributedTypeException ex = Assert.Throws<AttributedTypeException>(() => DbAttributAnalyser.Analyse(typeof(DbColumnIdentityNotUniqueClass)));
            Assert.That(ex.Message, Is.EqualTo("DbColumnAttribute identity could only be declared one time for the type"));
        }

        [DbTable]
        private class DbClass1
        {
            [DbColumn()]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
            [DbColumn(Name = "OverrideName")]
            public string Col3 { get; set; }
        }
        [DbTable(Name = "Table")]
        [DbRestictedDml(Restriction.Insert | Restriction.Delete)]
        private class DbClass2
        {
            [DbColumn(Kind = ColumnKind.Identity)]
            public string Col1 { get; set; }
            [DbColumn()]
            public string Col2 { get; set; }
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col3 { get; set; }
            [DbColumn(Kind = ColumnKind.PrimaryKey)]
            public string Col4 { get; set; }
        }

        [Test]
        public void TestAnalyse1()
        {
            TypeDbInfo typeDbInfo = DbAttributAnalyser.Analyse(typeof(DbClass1));
            Assert.IsNotNull(typeDbInfo, "typeDbInfo is null");
            Assert.AreEqual(typeDbInfo.TableName, nameof(DbClass1), "Not the expected TableName");

            Assert.AreEqual(3, typeDbInfo.Columns.Count, "Not the expected Columns Count");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass1.Col1)), "Col1 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass1.Col2)), "Col2 should be present");
            Assert.That(!typeDbInfo.Columns.ContainsKey(nameof(DbClass1.Col3)), "Col3 should not be present");
            Assert.That(typeDbInfo.Columns.ContainsKey("OverrideName"), "OverrideName should be present");
            
            Type t = typeof(DbClass1);
            Assert.AreEqual(t.GetProperty(nameof(DbClass1.Col1)), typeDbInfo.Columns[nameof(DbClass1.Col1)], "Not the expected value for Col1");
            Assert.AreEqual(t.GetProperty(nameof(DbClass1.Col2)), typeDbInfo.Columns[nameof(DbClass1.Col2)], "Not the expected value for Col2");
            Assert.AreEqual(t.GetProperty(nameof(DbClass1.Col3)), typeDbInfo.Columns["OverrideName"], "Not the expected value for Col2");
            Assert.IsNull(typeDbInfo.Identity, "Identity should be null");
            Assert.AreEqual(0, typeDbInfo.Keys.Count, "Not the expected Keys Count");
            Assert.AreEqual(Restriction.None, typeDbInfo.Restriction, "Not the expected Restriction");
        }

        [Test]
        public void TestAnalyse2()
        {
            TypeDbInfo typeDbInfo = DbAttributAnalyser.Analyse(typeof(DbClass2));
            Assert.IsNotNull(typeDbInfo, "typeDbInfo is null");

            Assert.AreEqual(typeDbInfo.TableName, "Table", "Not the expected TableName");
            Assert.AreEqual(4, typeDbInfo.Columns.Count, "Not the expected Columns Count");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col1)), "Col1 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col2)), "Col2 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col3)), "Col3 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col4)), "Col4 should be present");

            Type t = typeof(DbClass2);
            Assert.AreEqual(t.GetProperty(nameof(DbClass2.Col1)), typeDbInfo.Columns[nameof(DbClass2.Col1)], "Not the expected value for Col1");
            Assert.AreEqual(t.GetProperty(nameof(DbClass2.Col2)), typeDbInfo.Columns[nameof(DbClass2.Col2)], "Not the expected value for Col2");
            Assert.AreEqual(t.GetProperty(nameof(DbClass2.Col3)), typeDbInfo.Columns[nameof(DbClass2.Col3)], "Not the expected value for Col3");
            Assert.AreEqual(t.GetProperty(nameof(DbClass2.Col4)), typeDbInfo.Columns[nameof(DbClass2.Col4)], "Not the expected value for Col4");

            Assert.AreEqual(typeDbInfo.Identity, nameof(DbClass2.Col1), "Not the expected Identity");
            Assert.AreEqual(3, typeDbInfo.Keys.Count, "Not the expected Keys Count");
            CollectionAssert.AreEqual(new[] { nameof(DbClass2.Col1), nameof(DbClass2.Col3), nameof(DbClass2.Col4) }, typeDbInfo.Keys, "Not the expected value for Keys");
            Assert.AreEqual(Restriction.Insert | Restriction.Delete, typeDbInfo.Restriction, "Not the expected Restriction");
        }
    }
}