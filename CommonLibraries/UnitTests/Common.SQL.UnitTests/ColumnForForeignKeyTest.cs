namespace Common.SQL.UnitTests
{
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class ColumnForForeignKeyTest
    {
        [Test]
        public void TestProperties()
        {
            ColumnForForeignKey columnForForeignKey = new ColumnForForeignKey
            {
                Position = 10
            };
            Assert.That(columnForForeignKey.Position, Is.EqualTo(10));

            Column source = new Column();
            columnForForeignKey.SourceColumn = source;
            Assert.That(columnForForeignKey.SourceColumn, Is.EqualTo(source));

            Column reference = new Column();
            columnForForeignKey.ReferenceColumn = reference;
            Assert.That(columnForForeignKey.ReferenceColumn, Is.EqualTo(reference));
        }
        [Test]
        public void TestToString()
        {
            ColumnForForeignKey columnForForeignKey = new ColumnForForeignKey
            {
                SourceColumn = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "Name", CaseSensitivity = new CaseSensitivity(true) },
                Position = 10,
                ReferenceColumn = new Column { SchemaName = "SchemaName2", TableName = "TableName2", Name = "Name2", CaseSensitivity = new CaseSensitivity(true) }
            };
            Assert.That(columnForForeignKey.ToString(), Is.EqualTo("10 SchemaName.TableName.Name -> SchemaName2.TableName2.Name2"));
        }
        [TestCaseSource(nameof(TestCompareSource), new object[] { nameof(TestCompare) })]
        public void TestCompare(object columnForForeignKey, object columnForForeignKey2, IConstraint constraint)
        {
            Assert.That(((ColumnForForeignKey) columnForForeignKey).CompareTo(((ColumnForForeignKey) columnForForeignKey2)), constraint);
        }
        public static IEnumerable<TestCaseData> TestCompareSource(string methodCaller)
        {
            ColumnForForeignKey columnForForeignKey = new ColumnForForeignKey { Position = 1 };
            ColumnForForeignKey columnForForeignKey2 = new ColumnForForeignKey { Position = 1 };
            yield return new TestCaseData(columnForForeignKey, columnForForeignKey2, Is.EqualTo(0)).SetName($"{methodCaller} (Same Position)");

            columnForForeignKey = new ColumnForForeignKey { Position = 0 };
            columnForForeignKey2 = new ColumnForForeignKey { Position = 1 };
            yield return new TestCaseData(columnForForeignKey, columnForForeignKey2, Is.LessThan(0)).SetName($"{methodCaller} (different)");
        }
    }
}