namespace Common.SQL.UnitTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    using Index = Index;

    [TestFixture]
    public class IndexTest
    {
        [Test]
        public void TestProperties()
        {
            Index index = new Index
            {
                Name = "Name"
            };
            Assert.That(index.Name, Is.EqualTo("Name"));
            index.TableName = "TableName";
            Assert.That(index.TableName, Is.EqualTo("TableName"));
            index.SchemaName = "SchemaName";
            Assert.That(index.SchemaName, Is.EqualTo("SchemaName"));
            index.IsUnique = true;
            Assert.That(index.IsUnique, Is.True);
            index.IsClustered = true;
            Assert.That(index.IsClustered, Is.True);

            CaseSensitivity cs = new CaseSensitivity(false);
            index.CaseSensitivity = cs;
            Assert.That(index.CaseSensitivity, Is.EqualTo(cs));
        }
        [Test]
        public void TestAddColumnNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("column"), () => new Index().AddColumn(0, null));
        }
        [Test]
        public void TestAddColumnDifferentSchema()
        {
            Index index = new Index { TableName = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Wrong schema"),
                () => index.AddColumn(0, new Column { SchemaName = "SchemaName2", TableName = "TableName", Name = "Name", CaseSensitivity = index.CaseSensitivity }));
        }
        [Test]
        public void TestAddColumnDifferentTable()
        {
            Index index = new Index { TableName = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Wrong table"),
                () => index.AddColumn(0, new Column { SchemaName = "SchemaName", TableName = "TableName2", Name = "Name", CaseSensitivity = index.CaseSensitivity }));
        }
        [Test]
        public void TestColumns()
        {
            Index index = new Index { TableName = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Assert.That(index.Columns(), Is.Not.Null);
            Assert.That(index.Columns().Length, Is.EqualTo(0));

            index.AddColumn(1, new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = index.CaseSensitivity });
            Assert.That(index.Columns(), Is.Not.Null);
            Assert.That(index.Columns().Length, Is.EqualTo(1));

            index.AddColumn(0, new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name2", CaseSensitivity = index.CaseSensitivity });
            Assert.That(index.Columns(), Is.Not.Null);
            Assert.That(index.Columns().Length, Is.EqualTo(2));

            index.AddColumn(0, new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name3", CaseSensitivity = index.CaseSensitivity });
            Assert.That(index.Columns(), Is.Not.Null);
            Assert.That(index.Columns().Length, Is.EqualTo(2));
        }

        [TestCaseSource(nameof(TestToStringSource), new object[] { nameof(TestToString) })]
        public string TestToString(object index)
        {
            return index.ToString();
        }
        public static IEnumerable<TestCaseData> TestToStringSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                Index index = new Index { TableName = "TableName", SchemaName = "SchemaName", Name = "Name", IsClustered = true, IsUnique = true, CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(index).Returns(caseSensivityTestCaseSource.Expected("SchemaName.TableName.Name")).SetName($"{methodCaller} (Full {label})");

                index = new Index { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(index).Returns(caseSensivityTestCaseSource.Expected("TableName.Name")).SetName($"{methodCaller} (No schema {label})");

                index = new Index { SchemaName = "SchemaName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(index).Returns(caseSensivityTestCaseSource.Expected("SchemaName..Name")).SetName($"{methodCaller} (No table {label})");
            }
        }

        [TestCaseSource(nameof(TestCompareSource), new object[] { nameof(TestCompare) })]
        public void TestCompare(object index, object index2, IConstraint constraint)
        {
            Assert.That(((Index) index).CompareTo(((Index) index2)), constraint);
        }
        public static IEnumerable<TestCaseData> TestCompareSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                Index index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                Index index2 = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical with shema {label})");

                index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", IsClustered = true, IsUnique = true, CaseSensitivity = caseSensitivity };
                index2 = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical with shema ignore other property {label})");

                index = new Index { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                index2 = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.LessThan(0)).SetName($"{methodCaller} (no schema vs schema {label})");

                index = new Index { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                index2 = new Index { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical no schema {label})");

                index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                index2 = new Index { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.GreaterThan(0)).SetName($"{methodCaller} (schema vs no schema {label})");

                index = new Index { SchemaName = "SchemaName2", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                index2 = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.GreaterThan(0)).SetName($"{methodCaller} (different schema {label})");

                index = new Index { TableName = "TableName2", Name = "Name", CaseSensitivity = caseSensitivity };
                index2 = new Index { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.GreaterThan(0)).SetName($"{methodCaller} (different table {label})");

                index = new Index { TableName = "TableName", Name = "Name2", CaseSensitivity = caseSensitivity };
                index2 = new Index { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(index);
                yield return new TestCaseData(index, index2, Is.GreaterThan(0)).SetName($"{methodCaller} (same table different name {label})");
            }
        }
    }
}