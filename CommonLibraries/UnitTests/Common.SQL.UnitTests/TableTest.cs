namespace Common.SQL.UnitTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Index = Index;

    [TestFixture]
    public class TableTest
    {
        [Test]
        public void TestProperties()
        {
            Table table = new Table
            {
                Name = "Name"
            };
            Assert.That(table.Name, Is.EqualTo("Name"));

            table.SchemaName = "SchemaName";
            Assert.That(table.SchemaName, Is.EqualTo("SchemaName"));

            CaseSensitivity cs = new CaseSensitivity(false);
            table.CaseSensitivity = cs;
            Assert.That(table.CaseSensitivity, Is.EqualTo(cs));
        }
        [TestCaseSource(nameof(TestToStringSource), new object[] { nameof(TestToString) })]
        public string TestToString(object table)
        {
            return table.ToString();
        }
        public static IEnumerable<TestCaseData> TestToStringSource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                Table table = new Table { SchemaName = "SchemaName", Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(table).Returns(caseSensivityTestCaseSource.Expected("SchemaName.Name")).SetName($"{methodCaller} (Full {label})");

                table = new Table { Name = "Name", CaseSensitivity = caseSensitivity };
                yield return new TestCaseData(table).Returns(caseSensivityTestCaseSource.Expected("Name")).SetName($"{methodCaller} (no schema {label})");
            }
        }

        [TestCaseSource(nameof(TestTableKeySource), new object[] { nameof(TestTableKey) })]
        public string TestTableKey(string schemaName, string name, CaseSensitivity caseSensitivity)
        {
            return Table.TableKey(schemaName, name, caseSensitivity);
        }
        public static IEnumerable<TestCaseData> TestTableKeySource(string methodCaller)
        {
            foreach (CaseSensitivityTestCaseSource caseSensivityTestCaseSource in CaseSensitivityTestCaseSource.GetTestCases())
            {
                CaseSensitivity caseSensitivity = caseSensivityTestCaseSource.CaseSensitivity;
                string label = caseSensivityTestCaseSource.Label;

                yield return new TestCaseData("SchemaName", "Name", caseSensitivity).Returns(caseSensivityTestCaseSource.Expected("SchemaName.Name")).SetName($"{methodCaller} (schema + name {label})");
                yield return new TestCaseData(null, "Name", caseSensitivity).Returns(caseSensivityTestCaseSource.Expected("Name")).SetName($"{methodCaller} (name only {label})");
            }
        }

        [Test]
        public void TestSetPrimaryKeyNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("primaryKey"), () => new Table().SetPrimaryKey(null));
        }
        [Test]
        public void TestSetPrimaryKeyDifferentSchema()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName2", TableName = "TableName", Name = "PrimaryKeyName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("primaryKey").And.Message.StartsWith("Primary Key doesn't belong to table"), () => table.SetPrimaryKey(primaryKey));
        }
        [Test]
        public void TestSetPrimaryKeyDifferentTable()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName2", Name = "PrimaryKeyName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("primaryKey").And.Message.StartsWith("Primary Key doesn't belong to table"), () => table.SetPrimaryKey(primaryKey));
        }
        [Test]
        public void TestSetPrimaryKeyWithNoColumn()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "PrimaryKeyName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("primaryKey").And.Message.StartsWith("Empty primary key"), () => table.SetPrimaryKey(primaryKey));
        }
        [Test]
        public void TestSetPrimaryKeyWithColumnNoPresentInTable()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "PrimaryKeyName", CaseSensitivity = table.CaseSensitivity };
            primaryKey.AddColumn(0, new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity });
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("primaryKey").And.Message.StartsWith("Invalid column in primary key"), () => table.SetPrimaryKey(primaryKey));
        }
        [Test]
        public void TestSetPrimaryKey()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);
            PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "PrimaryKeyName", CaseSensitivity = table.CaseSensitivity };
            primaryKey.AddColumn(0, column);
            Assert.That(table.PrimaryKey, Is.Null);

            table.SetPrimaryKey(primaryKey);
            Assert.That(table.PrimaryKey, Is.EqualTo(primaryKey));
        }
        [Test]
        public void TestSetPrimaryKeyAlreadySet()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);
            PrimaryKey primaryKey = new PrimaryKey { SchemaName = "SchemaName", TableName = "TableName", Name = "PrimaryKeyName", CaseSensitivity = table.CaseSensitivity };
            primaryKey.AddColumn(0, column);

            table.SetPrimaryKey(primaryKey);

            Assert.Throws(Is.TypeOf<Exception>().With.Message.StartsWith("PrimaryKey is already set"), () => table.SetPrimaryKey(primaryKey));
        }

        [Test]
        public void TestAddIndexNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("index"), () => new Table().AddIndex(null));
        }
        [Test]
        public void TestAddIndexDifferentSchema()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Index index = new Index { SchemaName = "SchemaName2", TableName = "TableName", Name = "IndexName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("index").And.Message.StartsWith("Index doesn't belong to table"), () => table.AddIndex(index));
        }
        [Test]
        public void TestAddIndexDifferentTable()
        {

            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Index index = new Index { SchemaName = "SchemaName", TableName = "TableName2", Name = "IndexName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("index").And.Message.StartsWith("Index doesn't belong to table"), () => table.AddIndex(index));
        }
        [Test]
        public void TestAddIndexWithNoColumn()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Index index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "IndexName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("index").And.Message.StartsWith("Empty index"), () => table.AddIndex(index));
        }
        [Test]
        public void TestAddIndexWithColumnNoPresentInTable()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Index index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "IndexName", CaseSensitivity = table.CaseSensitivity };

            index.AddColumn(0, new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity });
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("index").And.Message.StartsWith("Invalid column in index"), () => table.AddIndex(index));
        }
        [Test]
        public void TestAddIndex()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);
            Index index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "IndexName", CaseSensitivity = table.CaseSensitivity };
            index.AddColumn(0, column);
            Assert.That(table.Indexes().Length, Is.EqualTo(0));

            table.AddIndex(index);
            Assert.That(table.Indexes().Length, Is.EqualTo(1));
        }
        [Test]
        public void TestAddIndexAlreadyPresent()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);
            Index index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "IndexName", CaseSensitivity = table.CaseSensitivity };
            index.AddColumn(0, column);

            table.AddIndex(index);

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("index").And.Message.StartsWith("Index already present"), () => table.AddIndex(index));
        }
        [Test]
        public void TestGetIndexHasIndex()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);
            Index index = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "IndexName", CaseSensitivity = table.CaseSensitivity };
            index.AddColumn(0, column);
            Index index2 = new Index { SchemaName = "SchemaName", TableName = "TableName", Name = "IndexName2", CaseSensitivity = table.CaseSensitivity };
            index2.AddColumn(0, column, true);
            Assert.That(table.HasIndex(index.Name), Is.False);
            Assert.That(table.GetIndex(index.Name), Is.Null);
            Assert.That(table.HasIndex(index2.Name), Is.False);
            Assert.That(table.GetIndex(index2.Name), Is.Null);
            Assert.That(table.HasIndex("aaaa"), Is.False);
            Assert.That(table.GetIndex("aaaa"), Is.Null);

            table.AddIndex(index);
            Assert.That(table.HasIndex(index.Name), Is.True);
            Assert.That(table.GetIndex(index.Name), Is.EqualTo(index));
            Assert.That(table.HasIndex(index2.Name), Is.False);
            Assert.That(table.GetIndex(index2.Name), Is.Null);
            Assert.That(table.HasIndex("aaaa"), Is.False);
            Assert.That(table.GetIndex("aaaa"), Is.Null);

            table.AddIndex(index2);
            Assert.That(table.HasIndex(index.Name), Is.True);
            Assert.That(table.GetIndex(index.Name), Is.EqualTo(index));
            Assert.That(table.HasIndex(index2.Name), Is.True);
            Assert.That(table.GetIndex(index2.Name), Is.EqualTo(index2));
            Assert.That(table.HasIndex("aaaa"), Is.False);
            Assert.That(table.GetIndex("aaaa"), Is.Null);
        }

        [Test]
        public void TestAddForeignKeyNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("foreignKey"), () => new Table().AddForeignKey(null));
        }
        [Test]
        public void TestAddForeignKeyDifferentSchema()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            ForeignKey foreignKey = new ForeignKey { Name = "ForeignKeyName", SourceSchemaName = "Schema2", SourceTableName = "TableName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("foreignKey").And.Message.StartsWith("ForeignKey doesn't belong to table"), () => table.AddForeignKey(foreignKey));
        }
        [Test]
        public void TestAddForeignKeyDifferentTable()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            ForeignKey foreignKey = new ForeignKey { Name = "ForeignKeyName", SourceSchemaName = "SchemaName", SourceTableName = "TableName2", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("foreignKey").And.Message.StartsWith("ForeignKey doesn't belong to table"), () => table.AddForeignKey(foreignKey));
        }
        [Test]
        public void TestAddForeignKeyWithNoColumn()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            ForeignKey foreignKey = new ForeignKey { Name = "ForeignKeyName", SourceSchemaName = "SchemaName", SourceTableName = "TableName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("foreignKey").And.Message.StartsWith("Empty foreignKey"), () => table.AddForeignKey(foreignKey));
        }
        [Test]
        public void TestAddForeignKeyWithColumnNoPresentInTable()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            ForeignKey foreignKey = new ForeignKey { Name = "ForeignKeyName", SourceSchemaName = "SchemaName", SourceTableName = "TableName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };
            Column sourceColumn = new Column { Name = "ColumnName", TableName = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column referenceColumn = new Column { Name = "ReferenceColumnName", TableName = "ReferenceTableName", SchemaName = "ReferenceSchemaName", CaseSensitivity = new CaseSensitivity(true) };
            foreignKey.AddColumn(0, sourceColumn, referenceColumn);

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("foreignKey").And.Message.StartsWith("Invalid column in foreignKey"), () => table.AddForeignKey(foreignKey));
        }
        [Test]
        public void TestAddForeignKey()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);

            ForeignKey foreignKey = new ForeignKey { Name = "ForeignKeyName", SourceSchemaName = "SchemaName", SourceTableName = "TableName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };
            Column referenceColumn = new Column { Name = "ReferenceColumnName", TableName = "ReferenceTableName", SchemaName = "ReferenceSchemaName", CaseSensitivity = new CaseSensitivity(true) };
            foreignKey.AddColumn(0, column, referenceColumn);

            Assert.That(table.ForeignKeys().Length, Is.EqualTo(0));

            table.AddForeignKey(foreignKey);
            Assert.That(table.ForeignKeys().Length, Is.EqualTo(1));
        }
        [Test]
        public void TestAddForeignKeyAlreadyPresent()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);

            ForeignKey foreignKey = new ForeignKey { Name = "ForeignKeyName", SourceSchemaName = "SchemaName", SourceTableName = "TableName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };
            Column referenceColumn = new Column { Name = "ReferenceColumnName", TableName = "ReferenceTableName", SchemaName = "ReferenceSchemaName", CaseSensitivity = new CaseSensitivity(true) };
            foreignKey.AddColumn(0, column, referenceColumn);

            table.AddForeignKey(foreignKey);

            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("foreignKey").And.Message.StartsWith("ForeignKey already present"), () => table.AddForeignKey(foreignKey));
        }
        [Test]
        public void TestGetForeignKeyHasForeignKey()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);

            ForeignKey foreignKey = new ForeignKey { Name = "ForeignKeyName", SourceSchemaName = "SchemaName", SourceTableName = "TableName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };
            Column referenceColumn = new Column { Name = "ReferenceColumnName", TableName = "ReferenceTableName", SchemaName = "ReferenceSchemaName", CaseSensitivity = new CaseSensitivity(true) };
            foreignKey.AddColumn(0, column, referenceColumn);

            ForeignKey foreignKey2 = new ForeignKey { Name = "ForeignKeyName2", SourceSchemaName = "SchemaName", SourceTableName = "TableName", ReferenceSchemaName = "ReferenceSchemaName", ReferenceTableName = "ReferenceTableName", CaseSensitivity = table.CaseSensitivity };
            Column referenceColumn2 = new Column { Name = "ReferenceColumnName2", TableName = "ReferenceTableName", SchemaName = "ReferenceSchemaName", CaseSensitivity = new CaseSensitivity(true) };
            foreignKey2.AddColumn(0, column, referenceColumn2);

            Assert.That(table.HasForeignKey(foreignKey.Name), Is.False);
            Assert.That(table.GetForeignKey(foreignKey.Name), Is.Null);
            Assert.That(table.HasForeignKey(foreignKey2.Name), Is.False);
            Assert.That(table.GetForeignKey(foreignKey2.Name), Is.Null);
            Assert.That(table.HasForeignKey("aaaa"), Is.False);
            Assert.That(table.GetForeignKey("aaaa"), Is.Null);

            table.AddForeignKey(foreignKey);
            Assert.That(table.HasForeignKey(foreignKey.Name), Is.True);
            Assert.That(table.GetForeignKey(foreignKey.Name), Is.EqualTo(foreignKey));
            Assert.That(table.HasForeignKey(foreignKey2.Name), Is.False);
            Assert.That(table.GetForeignKey(foreignKey2.Name), Is.Null);
            Assert.That(table.HasForeignKey("aaaa"), Is.False);
            Assert.That(table.GetForeignKey("aaaa"), Is.Null);

            table.AddForeignKey(foreignKey2);
            Assert.That(table.HasForeignKey(foreignKey.Name), Is.True);
            Assert.That(table.GetForeignKey(foreignKey.Name), Is.EqualTo(foreignKey));
            Assert.That(table.HasForeignKey(foreignKey2.Name), Is.True);
            Assert.That(table.GetForeignKey(foreignKey2.Name), Is.EqualTo(foreignKey2));
            Assert.That(table.HasForeignKey("aaaa"), Is.False);
            Assert.That(table.GetForeignKey("aaaa"), Is.Null);
        }

        [Test]
        public void TestAddColumnNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("column"), () => new Table().AddColumn(null));
        }
        [Test]
        public void TestAddColumnDifferentSchema()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { SchemaName = "SchemaName2", TableName = "TableName", Name = "ColumnName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Column doesn't belong to table"), () => table.AddColumn(column));
        }
        [Test]
        public void TestAddColumnDifferentTable()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { SchemaName = "SchemaName", TableName = "TableName2", Name = "ColumnName", CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Column doesn't belong to table"), () => table.AddColumn(column));
        }
        [Test]
        public void TestAddColumn()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName", Position = 0, CaseSensitivity = table.CaseSensitivity };
            Assert.That(table.Columns().Length, Is.EqualTo(0));

            table.AddColumn(column);
            Assert.That(table.Columns().Length, Is.EqualTo(1));

            Column column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName2", Position = 1, CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column2);
            Assert.That(table.Columns().Length, Is.EqualTo(2));
        }
        [Test]
        public void TestAddColumnWithNameAlreadyPresent()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName", Position = 0, CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);

            Column column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName", Position = 1, CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Column name already present"), () => table.AddColumn(column2));
        }
        [Test]
        public void TestAddColumnWithPositionAlreadyPresent()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName", Position = 0, CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column);

            Column column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName2", Position = 0, CaseSensitivity = table.CaseSensitivity };
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("column").And.Message.StartsWith("Column position already present"), () => table.AddColumn(column2));
        }
        [Test]
        public void TestColumnsIsOrdered()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName", Position = 1, CaseSensitivity = table.CaseSensitivity };
            Assert.That(table.Columns().Length, Is.EqualTo(0));

            table.AddColumn(column);
            Assert.That(table.Columns().Length, Is.EqualTo(1));
            Assert.That(table.Columns()[0], Is.EqualTo(column));

            Column column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName2", Position = 0, CaseSensitivity = table.CaseSensitivity };
            table.AddColumn(column2);
            Assert.That(table.Columns().Length, Is.EqualTo(2));
            Assert.That(table.Columns()[0], Is.EqualTo(column2));
            Assert.That(table.Columns()[1], Is.EqualTo(column));
        }
        [Test]
        public void TestGetColumnHasColumn()
        {
            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = new CaseSensitivity(true) };
            Column column = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName", Position = 1, CaseSensitivity = table.CaseSensitivity };
            Column column2 = new Column { SchemaName = "SchemaName", TableName = "TableName", Name = "ColumnName2", Position = 0, CaseSensitivity = table.CaseSensitivity };

            Assert.That(table.HasColumn(column.Name), Is.False);
            Assert.That(table.GetColumn(column.Name), Is.Null);
            Assert.That(table.HasColumn(column2.Name), Is.False);
            Assert.That(table.GetColumn(column2.Name), Is.Null);
            Assert.That(table.HasColumn("aaaa"), Is.False);
            Assert.That(table.GetColumn("aaaa"), Is.Null);

            table.AddColumn(column);
            Assert.That(table.HasColumn(column.Name), Is.True);
            Assert.That(table.GetColumn(column.Name), Is.EqualTo(column));
            Assert.That(table.HasColumn(column2.Name), Is.False);
            Assert.That(table.GetColumn(column2.Name), Is.Null);
            Assert.That(table.HasColumn("aaaa"), Is.False);
            Assert.That(table.GetColumn("aaaa"), Is.Null);

            table.AddColumn(column2);
            Assert.That(table.HasColumn(column.Name), Is.True);
            Assert.That(table.GetColumn(column.Name), Is.EqualTo(column));
            Assert.That(table.HasColumn(column2.Name), Is.True);
            Assert.That(table.GetColumn(column2.Name), Is.EqualTo(column2));
            Assert.That(table.HasColumn("aaaa"), Is.False);
            Assert.That(table.GetColumn("aaaa"), Is.Null);
        }
    }
}