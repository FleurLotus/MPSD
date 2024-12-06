namespace Common.SQL.UnitTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class ForeignKeyTest
    {
        [Test]
        public void TestProperties()
        {
            ForeignKey foreignKey = new ForeignKey();

            foreignKey.Name = "Name";
            Assert.That(foreignKey.Name, Is.EqualTo("Name"));
            foreignKey.SourceTableName = "SourceTableName";
            Assert.That(foreignKey.SourceTableName, Is.EqualTo("SourceTableName"));
            foreignKey.SourceSchemaName = "SourceSchemaName";
            Assert.That(foreignKey.SourceSchemaName, Is.EqualTo("SourceSchemaName"));

            foreignKey.ReferenceTableName = "ReferenceTableName";
            Assert.That(foreignKey.ReferenceTableName, Is.EqualTo("ReferenceTableName"));
            foreignKey.ReferenceSchemaName = "ReferenceSchemaName";
            Assert.That(foreignKey.ReferenceSchemaName, Is.EqualTo("ReferenceSchemaName"));

            foreignKey.UpdateRule = "UpdateRule";
            Assert.That(foreignKey.UpdateRule, Is.EqualTo("UpdateRule"));
            foreignKey.DeleteRule = "DeleteRule";
            Assert.That(foreignKey.DeleteRule, Is.EqualTo("DeleteRule"));

            CaseSensitivity cs = new CaseSensitivity(false);
            foreignKey.CaseSensitivity = cs;
            Assert.That(foreignKey.CaseSensitivity, Is.EqualTo(cs));
        }
        [Test]
        public void TestAddColumnSourceNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("source"), () => new ForeignKey().AddColumn(0, null, new Column()));
        }
        [Test]
        public void TestAddColumnReferenceNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("reference"), () => new ForeignKey().AddColumn(0, new Column(), null));
        }
        [Test]
        public void TestAddColumnSourceDifferentSchema()
        {
            ForeignKey foreignKey = new ForeignKey { SourceTableName = "SourceTableName", SourceSchemaName = "SourceSchemaName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = new CaseSensitivity(true) };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("source").And.Message.StartsWith("Wrong source schema"),
                    () => foreignKey.AddColumn(0,
                        new Column { SchemaName = "SourceSchemaName2", TableName = "SourceTableName", Name = "SourceName", CaseSensitivity = foreignKey.CaseSensitivity },
                        new Column { SchemaName = "ReferenceSchemaName", TableName = "ReferenceTableName", Name = "ReferenceName", CaseSensitivity = foreignKey.CaseSensitivity }
                    ));
        }
        [Test]
        public void TestAddColumnSourceDifferentTable()
        {
            ForeignKey foreignKey = new ForeignKey { SourceTableName = "SourceTableName", SourceSchemaName = "SourceSchemaName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = new CaseSensitivity(true) };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("source").And.Message.StartsWith("Wrong source table"),
                    () => foreignKey.AddColumn(0,
                        new Column { SchemaName = "SourceSchemaName", TableName = "SourceTableName2", Name = "SourceName", CaseSensitivity = foreignKey.CaseSensitivity },
                        new Column { SchemaName = "ReferenceSchemaName", TableName = "ReferenceTableName", Name = "ReferenceName", CaseSensitivity = foreignKey.CaseSensitivity }
                    ));
        }
        [Test]
        public void TestAddColumnReferenceDifferentSchema()
        {
            ForeignKey foreignKey = new ForeignKey { SourceTableName = "SourceTableName", SourceSchemaName = "SourceSchemaName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = new CaseSensitivity(true) };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("reference").And.Message.StartsWith("Wrong reference schema"),
                    () => foreignKey.AddColumn(0,
                        new Column { SchemaName = "SourceSchemaName", TableName = "SourceTableName", Name = "SourceName", CaseSensitivity = foreignKey.CaseSensitivity },
                        new Column { SchemaName = "ReferenceSchemaName2", TableName = "ReferenceTableName", Name = "ReferenceName", CaseSensitivity = foreignKey.CaseSensitivity }
                    ));
        }
        [Test]
        public void TestAddColumnReferenceDifferentTable()
        {
            ForeignKey foreignKey = new ForeignKey { SourceTableName = "SourceTableName", SourceSchemaName = "SourceSchemaName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = new CaseSensitivity(true) };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("reference").And.Message.StartsWith("Wrong reference table"),
                    () => foreignKey.AddColumn(0,
                        new Column { SchemaName = "SourceSchemaName", TableName = "SourceTableName", Name = "SourceName", CaseSensitivity = foreignKey.CaseSensitivity },
                        new Column { SchemaName = "ReferenceSchemaName", TableName = "ReferenceTableName2", Name = "ReferenceName", CaseSensitivity = foreignKey.CaseSensitivity }
                    ));
        }
        [Test]
        public void TestColumns()
        {
            ForeignKey foreignKey = new ForeignKey { SourceTableName = "SourceTableName", SourceSchemaName = "SourceSchemaName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = new CaseSensitivity(true) };

            Assert.That(foreignKey.Columns(), Is.Not.Null);
            Assert.That(foreignKey.Columns().Length, Is.EqualTo(0));

            foreignKey.AddColumn(1,
                    new Column { SchemaName = "SourceSchemaName", TableName = "SourceTableName", Name = "SourceName", CaseSensitivity = foreignKey.CaseSensitivity },
                    new Column { SchemaName = "ReferenceSchemaName", TableName = "ReferenceTableName", Name = "ReferenceName", CaseSensitivity = foreignKey.CaseSensitivity });

            Assert.That(foreignKey.Columns(), Is.Not.Null);
            Assert.That(foreignKey.Columns().Length, Is.EqualTo(1));

            foreignKey.AddColumn(0,
                new Column { SchemaName = "SourceSchemaName", TableName = "SourceTableName", Name = "SourceName2", CaseSensitivity = foreignKey.CaseSensitivity },
                new Column { SchemaName = "ReferenceSchemaName", TableName = "ReferenceTableName", Name = "ReferenceName2", CaseSensitivity = foreignKey.CaseSensitivity });
            Assert.That(foreignKey.Columns(), Is.Not.Null);
            Assert.That(foreignKey.Columns().Length, Is.EqualTo(2));


            foreignKey.AddColumn(0,
                new Column { SchemaName = "SourceSchemaName", TableName = "SourceTableName", Name = "SourceName3", CaseSensitivity = foreignKey.CaseSensitivity },
                new Column { SchemaName = "ReferenceSchemaName", TableName = "ReferenceTableName", Name = "ReferenceName3", CaseSensitivity = foreignKey.CaseSensitivity });
            Assert.That(foreignKey.Columns(), Is.Not.Null);
            Assert.That(foreignKey.Columns().Length, Is.EqualTo(2));
        }
        [TestCaseSource(nameof(TestToStringSource), new object[] { nameof(TestToString) })]
        public string TestToString(object foreignKey)
        {
            return foreignKey.ToString();
        }
        public static IEnumerable<TestCaseData> TestToStringSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                ForeignKey foreignKey = new ForeignKey { SourceTableName = "SourceTableName", SourceSchemaName = "SourceSchemaName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(foreignKey).Returns(caseSensivityTestCaseSource.Expected("SourceSchemaName.SourceTableName.Name")).SetName($"{methodCaller} (Full {label})");

                foreignKey = new ForeignKey { SourceTableName = "SourceTableName", SourceSchemaName = "SourceSchemaName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(foreignKey).Returns(caseSensivityTestCaseSource.Expected("SourceSchemaName.SourceTableName.Name")).SetName($"{methodCaller} (No reference {label})");

                foreignKey = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(foreignKey).Returns(caseSensivityTestCaseSource.Expected("SourceTableName.Name")).SetName($"{methodCaller} (No schema {label})");

                foreignKey = new ForeignKey { SourceSchemaName = "SourceSchemaName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(foreignKey).Returns(caseSensivityTestCaseSource.Expected("SourceSchemaName..Name")).SetName($"{methodCaller} (No table {label})");
            }
        }
        [TestCaseSource(nameof(TestCompareSource), new object[] { nameof(TestCompare) })]
        public void TestCompare(object foreignKey, object foreignKey2, IConstraint constraint)
        {
            Assert.That(((ForeignKey)foreignKey).CompareTo(((ForeignKey)foreignKey2)), constraint);
        }
        public static IEnumerable<TestCaseData> TestCompareSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                ForeignKey foreignKey = new ForeignKey { SourceSchemaName = "SourceSchemaName", SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                ForeignKey foreignKey2 = new ForeignKey { SourceSchemaName = "SourceSchemaName", SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical with shema {label})");

                foreignKey = new ForeignKey { SourceSchemaName = "SourceSchemaName", SourceTableName = "SourceTableName", Name = "Name", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", DeleteRule = "DeleteRule", UpdateRule = "UpdateRule", CaseSensitivity = caseSensitivity };
                foreignKey2 = new ForeignKey { SourceSchemaName = "SourceSchemaName", SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical with shema ignore other property {label})");

                foreignKey = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                foreignKey2 = new ForeignKey { SourceSchemaName = "SourceSchemaName", SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.LessThan(0)).SetName($"{methodCaller} (no schema vs schema {label})");

                foreignKey = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                foreignKey2 = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical no schema {label})");

                foreignKey = new ForeignKey { SourceSchemaName = "SourceSchemaName", SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                foreignKey2 = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (schema vs no schema {label})");

                foreignKey = new ForeignKey { SourceSchemaName = "SourceSchemaName2", SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                foreignKey2 = new ForeignKey { SourceSchemaName = "SourceSchemaName", SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (different schema {label})");

                foreignKey = new ForeignKey { SourceTableName = "SourceTableName2", Name = "Name", CaseSensitivity = caseSensitivity };
                foreignKey2 = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (different sourcetable {label})");

                foreignKey = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name2", CaseSensitivity = caseSensitivity };
                foreignKey2 = new ForeignKey { SourceTableName = "SourceTableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(foreignKey);
                yield return new TestCaseData(foreignKey, foreignKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (same sourcetable different name {label})");
            }
        }
    }
}