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
            [DbColumn(Kind = ColumnKind.Identity)]
            public string Col1 { get; set; }
            [DbColumn(Kind = ColumnKind.Identity)]
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
            Assert.That(typeDbInfo, Is.Not.Null, "typeDbInfo is null");
            Assert.That(typeDbInfo.TableName, Is.EqualTo(nameof(DbClass1)), "Not the expected TableName");

            Assert.That(typeDbInfo.Columns.Count, Is.EqualTo(3), "Not the expected Columns Count");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass1.Col1)), Is.True, "Col1 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass1.Col2)), Is.True, "Col2 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass1.Col3)), Is.False, "Col3 should not be present");
            Assert.That(typeDbInfo.Columns.ContainsKey("OverrideName"), Is.True, "OverrideName should be present");

            Type t = typeof(DbClass1);
            Assert.That(typeDbInfo.Columns[nameof(DbClass1.Col1)], Is.EqualTo(t.GetProperty(nameof(DbClass1.Col1))), "Not the expected value for Col1");
            Assert.That(typeDbInfo.Columns[nameof(DbClass1.Col2)], Is.EqualTo(t.GetProperty(nameof(DbClass1.Col2))), "Not the expected value for Col2");
            Assert.That(typeDbInfo.Columns["OverrideName"], Is.EqualTo(t.GetProperty(nameof(DbClass1.Col3))), "Not the expected value for Col2");
            Assert.That(typeDbInfo.Identity, Is.Null, "Identity should be null");
            Assert.That(typeDbInfo.Keys.Count, Is.EqualTo(0), "Not the expected Keys Count");
            Assert.That(typeDbInfo.Restriction, Is.EqualTo(Restriction.None), "Not the expected Restriction");
        }

        [Test]
        public void TestAnalyse2()
        {
            TypeDbInfo typeDbInfo = DbAttributAnalyser.Analyse(typeof(DbClass2));
            Assert.That(typeDbInfo, Is.Not.Null, "typeDbInfo is null");

            Assert.That(typeDbInfo.TableName, Is.EqualTo("Table"), "Not the expected TableName");
            Assert.That(typeDbInfo.Columns.Count, Is.EqualTo(4), "Not the expected Columns Count");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col1)), Is.True, "Col1 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col2)), Is.True, "Col2 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col3)), Is.True, "Col3 should be present");
            Assert.That(typeDbInfo.Columns.ContainsKey(nameof(DbClass2.Col4)), Is.True, "Col4 should be present");

            Type t = typeof(DbClass2);
            Assert.That(typeDbInfo.Columns[nameof(DbClass2.Col1)], Is.EqualTo(t.GetProperty(nameof(DbClass2.Col1))), "Not the expected value for Col1");
            Assert.That(typeDbInfo.Columns[nameof(DbClass2.Col2)], Is.EqualTo(t.GetProperty(nameof(DbClass2.Col2))), "Not the expected value for Col2");
            Assert.That(typeDbInfo.Columns[nameof(DbClass2.Col3)], Is.EqualTo(t.GetProperty(nameof(DbClass2.Col3))), "Not the expected value for Col3");
            Assert.That(typeDbInfo.Columns[nameof(DbClass2.Col4)], Is.EqualTo(t.GetProperty(nameof(DbClass2.Col4))), "Not the expected value for Col4");

            Assert.That(typeDbInfo.Identity, Is.EqualTo(nameof(DbClass2.Col1)), "Not the expected Identity");
            Assert.That(typeDbInfo.Keys.Count, Is.EqualTo(3), "Not the expected Keys Count");
            Assert.That(typeDbInfo.Keys, Is.EqualTo(new[] { nameof(DbClass2.Col1), nameof(DbClass2.Col3), nameof(DbClass2.Col4) }), "Not the expected value for Keys");
            Assert.That(typeDbInfo.Restriction, Is.EqualTo(Restriction.Insert | Restriction.Delete), "Not the expected Restriction");
        }
    }
}