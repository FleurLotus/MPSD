namespace Common.SQL.UnitTests
{
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class ColumnTest
    {
        [Test]
        public void TestProperties()
        {
            Column column = new Column
            {
                Name = "Name"
            };
            Assert.That(column.Name, Is.EqualTo("Name"));

            column.IsNullable = true;
            Assert.That(column.IsNullable, Is.True);

            column.DataType = "DataType";
            Assert.That(column.DataType, Is.EqualTo("DataType"));

            column.CharacterMaxLength = 10;
            Assert.That(column.CharacterMaxLength, Is.EqualTo(10));

            column.NumericPrecision = 5;
            Assert.That(column.NumericPrecision, Is.EqualTo(5));

            column.NumericScale = 2;
            Assert.That(column.NumericScale, Is.EqualTo(2));

            column.AutoIncrementBy = 1;
            Assert.That(column.AutoIncrementBy, Is.EqualTo(1));

            column.AutoIncrementSeed = 1;
            Assert.That(column.AutoIncrementSeed, Is.EqualTo(1));

            column.AutoIncrementNext = 3;
            Assert.That(column.AutoIncrementNext, Is.EqualTo(3));

            column.HasDefault = false;
            Assert.That(column.HasDefault, Is.False);

            column.Default = "Default";
            Assert.That(column.Default, Is.EqualTo("Default"));

            column.TableName = "TableName";
            Assert.That(column.TableName, Is.EqualTo("TableName"));

            column.SchemaName = "SchemaName";
            Assert.That(column.SchemaName, Is.EqualTo("SchemaName"));

            column.Position = 123;
            Assert.That(column.Position, Is.EqualTo(123));

            column.RowGuidCol = true;
            Assert.That(column.RowGuidCol, Is.True);

            column.Width = 12;
            Assert.That(column.Width, Is.EqualTo(12));

            column.PadLeft = false;
            Assert.That(column.PadLeft, Is.False);

            CaseSensitivity cs = new CaseSensitivity(false);
            column.CaseSensitivity = cs;
            Assert.That(column.CaseSensitivity, Is.EqualTo(cs));
        }

        [TestCaseSource(nameof(TestToStringSource), new object[] { nameof(TestToString) })]
        public string TestToString(object column)
        {
            return column.ToString();
        }

        public static IEnumerable<TestCaseData> TestToStringSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                Column column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(column).Returns(caseSensivityTestCaseSource.Expected("SchemaName.TableName.Name")).SetName($"{methodCaller} (Full {label})");

                column = new Column { TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(column).Returns(caseSensivityTestCaseSource.Expected("TableName.Name")).SetName($"{methodCaller} (no schema {label})");
            }
        }
        [TestCaseSource(nameof(TestCompareSource), new object[] { nameof(TestCompare) })]
        public void TestCompare(object column, object column2, IConstraint constraint)
        {
            Assert.That(((Column) column).CompareTo(((Column) column2)), constraint);
        }
        public static IEnumerable<TestCaseData> TestCompareSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                Column column = new Column { SchemaName = "SchemaName", TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                Column column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(column);
                yield return new TestCaseData(column, column2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical with shema {label})");

                column = new Column { TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(column);
                yield return new TestCaseData(column, column2, Is.LessThan(0)).SetName($"{methodCaller} (no schema vs schema {label})");

                column = new Column { TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                column2 = new Column { TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(column);
                yield return new TestCaseData(column, column2, Is.EqualTo(0)).SetName($"{methodCaller} (Identical no schema {label})");

                column = new Column { SchemaName = "SchemaName2", TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                column2 = new Column { TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(column);
                yield return new TestCaseData(column, column2, Is.GreaterThan(0)).SetName($"{methodCaller} (schema vs no schema {label})");

                column = new Column { SchemaName = "SchemaName2", TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(column);
                yield return new TestCaseData(column, column2, Is.GreaterThan(0)).SetName($"{methodCaller} (different schema {label})");

                column = new Column { TableName = "TableName2", Position = 1, CaseSensitivity = caseSensitivity };
                column2 = new Column { TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(column);
                yield return new TestCaseData(column, column2, Is.GreaterThan(0)).SetName($"{methodCaller} (different table {label})");

                column = new Column { TableName = "TableName", Position = 0, CaseSensitivity = caseSensitivity };
                column2 = new Column { TableName = "TableName", Position = 1, CaseSensitivity = caseSensitivity };
                caseSensivityTestCaseSource.Apply(column);
                yield return new TestCaseData(column, column2, Is.LessThan(0)).SetName($"{methodCaller} (same table different position {label})");
            }
        }
        [TestCaseSource(nameof(TestShortTypeSource), new object[] { nameof(TestShortType) })]
        public string TestShortType(object column)
        {
            return ((Column) column).ShortType;
        }
        public static IEnumerable<TestCaseData> TestShortTypeSource(string methodCaller)
        {
            Column column = new Column { DataType = "nvarchar", CharacterMaxLength = 10 };
            yield return new TestCaseData(column).Returns("nvarchar(10)").SetName($"{methodCaller} (nvarchar)");

            column = new Column { DataType = "nchar", CharacterMaxLength = 10 };
            yield return new TestCaseData(column).Returns("nchar(10)").SetName($"{methodCaller} (nchar)");

            column = new Column { DataType = "binary", CharacterMaxLength = 10 };
            yield return new TestCaseData(column).Returns("binary(10)").SetName($"{methodCaller} (binary)");

            column = new Column { DataType = "varbinary", CharacterMaxLength = 10 };
            yield return new TestCaseData(column).Returns("varbinary(10)").SetName($"{methodCaller} (varbinary)");

            column = new Column { DataType = "numeric", NumericPrecision = 25, NumericScale = 17 };
            yield return new TestCaseData(column).Returns("numeric(25,17)").SetName($"{methodCaller} (numeric)");

            column = new Column { DataType = "bool" };
            yield return new TestCaseData(column).Returns("bool").SetName($"{methodCaller} (bool)");
        }
    }
}