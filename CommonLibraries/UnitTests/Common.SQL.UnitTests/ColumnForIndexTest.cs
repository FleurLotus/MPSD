namespace Common.SQL.UnitTests
{
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class ColumnForIndexTest
    {
        [Test]
        public void TestProperties()
        {
            ColumnForIndex columnForIndex = new ColumnForIndex();
            columnForIndex.Position = 10;
            Assert.That(columnForIndex.Position, Is.EqualTo(10));

            Column source = new Column();
            columnForIndex.Column = source;
            Assert.That(columnForIndex.Column, Is.EqualTo(source));

            columnForIndex.IsAsc = true;
            Assert.That(columnForIndex.IsAsc, Is.True);
        }
        [TestCaseSource(nameof(TestToStringSource), new object[] { nameof(TestToString) })]
        public string TestToString(object columnForIndex)
        {
            return columnForIndex.ToString();
        }
        public static IEnumerable<TestCaseData> TestToStringSource(string methodCaller)
        {
            CaseSensitivity caseSensitivity = new CaseSensitivity(true);
            ColumnForIndex columnForIndex = new ColumnForIndex { Position = 10, Column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity }, IsAsc = true };
            yield return new TestCaseData(columnForIndex).Returns("10 SchemaName.TableName.Name Asc").SetName($"{methodCaller} (IsAsc = True)");

            columnForIndex = new ColumnForIndex { Position = 10, Column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity }, IsAsc = false };
            yield return new TestCaseData(columnForIndex).Returns("10 SchemaName.TableName.Name Desc").SetName($"{methodCaller} (IsAsc = False)");

            columnForIndex = new ColumnForIndex { Position = 10, Column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = caseSensitivity } };
            yield return new TestCaseData(columnForIndex).Returns("10 SchemaName.TableName.Name").SetName($"{methodCaller} (IsAsc = null)");
        }
        [TestCaseSource(nameof(TestCompareSource), new object[] { nameof(TestCompare) })]
        public void TestCompare(object columnForIndex, object columnForIndex2, IConstraint constraint)
        {
            Assert.That(((ColumnForIndex)columnForIndex).CompareTo(((ColumnForIndex)columnForIndex2)), constraint);
        }
        public static IEnumerable<TestCaseData> TestCompareSource(string methodCaller)
        {
            ColumnForIndex columnForIndex = new ColumnForIndex { Position = 1 };
            ColumnForIndex columnForIndex2 = new ColumnForIndex { Position = 1 };
            yield return new TestCaseData(columnForIndex, columnForIndex2, Is.EqualTo(0)).SetName($"{methodCaller} (Same Position)");

            columnForIndex = new ColumnForIndex { Position = 0 };
            columnForIndex2 = new ColumnForIndex { Position = 1 };
            yield return new TestCaseData(columnForIndex, columnForIndex2, Is.LessThan(0)).SetName($"{methodCaller} (different)");
        }
    }
}