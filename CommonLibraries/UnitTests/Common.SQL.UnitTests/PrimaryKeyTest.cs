namespace Common.SQL.UnitTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class PrimaryKeyTest
    {
        [Test]
        public void TestProperties()
        {
            PrimaryKey primaryKey = new PrimaryKey
            {
                Name = "Name"
            };
            Assert.That(primaryKey.Name, Is.EqualTo("Name"));

            primaryKey.TableName = "TableName";
            Assert.That(primaryKey.TableName, Is.EqualTo("TableName"));

            primaryKey.SchemaName = "SchemaName";
            Assert.That(primaryKey.SchemaName, Is.EqualTo("SchemaName"));

            CaseSensitivity cs = new CaseSensitivity(false);
            primaryKey.CaseSensitivity = cs;
            Assert.That(primaryKey.CaseSensitivity, Is.EqualTo(cs));
        }
        [Test]
        public void TestAddColumnNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("column"), () => new PrimaryKey().AddColumn(0, null));
        }
        [Test]
        public void TestAddColumnDifferentSchema()
        {
            PrimaryKey primaryKey = new PrimaryKey { TableName = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Wrong schema"),
                () => primaryKey.AddColumn(0, new Column { SchemaName = "SchemaName2", TableName = "TableName", Name = "Name", CaseSensitivity = primaryKey.CaseSensitivity }));
        }
        [Test]
        public void TestAddColumnDifferentTable()
        {
            PrimaryKey primaryKey = new PrimaryKey { TableName = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Wrong table"),
                () => primaryKey.AddColumn(0, new Column { SchemaName = "SchemaName", TableName = "TableName2", Name = "Name", CaseSensitivity = primaryKey.CaseSensitivity }));
        }
        [Test]
        public void TestColumns()
        {
            PrimaryKey primaryKey = new PrimaryKey();
            Assert.That(primaryKey.Columns(), Is.Not.Null);
            Assert.That(primaryKey.Columns().Length, Is.EqualTo(0));

            Column column = new Column();
            primaryKey.AddColumn(1, column);
            Assert.That(primaryKey.Columns(), Is.Not.Null);
            Assert.That(primaryKey.Columns().Length, Is.EqualTo(1));
            Assert.That(primaryKey.Columns()[0], Is.EqualTo(column));

            Column column2 = new Column();
            primaryKey.AddColumn(0, column2);
            Assert.That(primaryKey.Columns(), Is.Not.Null);
            Assert.That(primaryKey.Columns().Length, Is.EqualTo(2));
            Assert.That(primaryKey.Columns()[0], Is.EqualTo(column2));
            Assert.That(primaryKey.Columns()[1], Is.EqualTo(column));

            Column column3 = new Column();
            primaryKey.AddColumn(0, column3);
            Assert.That(primaryKey.Columns(), Is.Not.Null);
            Assert.That(primaryKey.Columns().Length, Is.EqualTo(2));
            Assert.That(primaryKey.Columns()[0], Is.EqualTo(column3));
            Assert.That(primaryKey.Columns()[1], Is.EqualTo(column));
        }
        [TestCaseSource(nameof(TestToStringSource), new object[] { nameof(TestToString) })]
        public string TestToString(object primaryKey)
        {
            return primaryKey.ToString();
        }
        public static IEnumerable<TestCaseData> TestToStringSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(primaryKey).Returns(caseSensivityTestCaseSource.Expected("SchemaName.TableName.Name")).SetName($"{methodCaller} (Full {label})");

                primaryKey = new PrimaryKey { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(primaryKey).Returns(caseSensivityTestCaseSource.Expected("TableName.Name")).SetName($"{methodCaller} (no schema {label})");

                primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(primaryKey).Returns(caseSensivityTestCaseSource.Expected("SchemaName.TableName.*AUTO*")).SetName($"{methodCaller} (no name {label})");

                primaryKey = new PrimaryKey { TableName = "TableName", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(primaryKey).Returns(caseSensivityTestCaseSource.Expected("TableName.*AUTO*")).SetName($"{methodCaller} (no schema, no name {label})");
            }
        }
        [TestCaseSource(nameof(TestCompareSource), new object[] { nameof(TestCompare) })]
        public void TestCompare(object primaryKey, object primaryKey2, IConstraint constraint)
        {
            Assert.That(((PrimaryKey) primaryKey).CompareTo(((PrimaryKey) primaryKey2)), constraint);
        }
        public static IEnumerable<TestCaseData> TestCompareSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                PrimaryKey primaryKey2 = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(primaryKey);
                yield return new TestCaseData(primaryKey, primaryKey2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical with shema {label})");

                primaryKey = new PrimaryKey { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                primaryKey2 = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(primaryKey);
                yield return new TestCaseData(primaryKey, primaryKey2, Is.LessThan(0)).SetName($"{methodCaller} (no schema vs schema {label})");

                primaryKey = new PrimaryKey { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                primaryKey2 = new PrimaryKey { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(primaryKey);
                yield return new TestCaseData(primaryKey, primaryKey2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical no schema {label})");

                primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                primaryKey2 = new PrimaryKey { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(primaryKey);
                yield return new TestCaseData(primaryKey, primaryKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (schema vs no schema {label})");

                primaryKey = new PrimaryKey { SchemaName = "SchemaName2", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                primaryKey2 = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(primaryKey);
                yield return new TestCaseData(primaryKey, primaryKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (different schema {label})");

                primaryKey = new PrimaryKey { TableName = "TableName2", Name = "Name", CaseSensitivity = caseSensitivity };
                primaryKey2 = new PrimaryKey { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(primaryKey);
                yield return new TestCaseData(primaryKey, primaryKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (different table {label})");

                primaryKey = new PrimaryKey { TableName = "TableName", Name = "Name2", CaseSensitivity = caseSensitivity };
                primaryKey2 = new PrimaryKey { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(primaryKey);
                yield return new TestCaseData(primaryKey, primaryKey2, Is.GreaterThan(0)).SetName($"{methodCaller} (same table different name {label})");
            }
        }
    }
}