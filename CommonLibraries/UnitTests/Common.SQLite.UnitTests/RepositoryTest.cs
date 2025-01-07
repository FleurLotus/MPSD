namespace Common.SQLite.UnitTests
{
    using System;
    using System.IO;
    using System.Data.SQLite;

    using NUnit.Framework;

    using Common.SQL;

    [TestFixture]
    public class RepositoryTest
    {
        private const string SQL =
@"CREATE TABLE ""Table1"" (
    ""Field1"" INTEGER NULL,
    ""Field2"" TEXT NOT NULL,
    ""Field3"" REAL NULL,
    ""Field4"" INTEGER NULL,
    ""Field5"" BLOB NULL,
    ""Field6"" INTEGER NOT NULL DEFAULT 10,
    UNIQUE(""Field2""),
    PRIMARY KEY(""Field1"")
);

CREATE TABLE ""Table2"" (
    ""Field1"" INTEGER NOT NULL,
    ""Field2"" INTEGER NOT NULL, 
    ""Field3"" INTEGER NOT NULL,
    ""Field4"" INTEGER NOT NULL,
    ""Field5"" INTEGER NOT NULL,
    PRIMARY KEY(""Field2"", ""Field3""),
    FOREIGN KEY(""Field1"") REFERENCES Table1(Field1),
    FOREIGN KEY(""Field1"", ""Field3"") REFERENCES Table1(Field1, Field6),
    UNIQUE(""Field4"", ""Field5"")
);";

        public string CreateDbFile()
        {
            string fileName = $"Sample_{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}.sqlite";
            SQLiteConnection.CreateFile(fileName);
            string connectionString = (new SQLiteConnectionStringBuilder { DataSource = fileName }).ToString(); 
            
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SQL, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            return fileName;
        }
        [Test]
        public void TestRepository()
        {
            string fileName = null;
            ITable[] tables;
            ITable table;
            IColumn[] columns;
            IColumn column;
            IPrimaryKey primaryKey;
            IIndex[] indexes;
            IIndex index;
            IColumnForIndex[] columnsForIndex;
            IColumnForIndex columnForIndex;
            IForeignKey[] foreignKeys;
            IForeignKey foreignKey;
            IColumnForForeignKey[] columnsForForeignKey;
            IColumnForForeignKey columnForForeignKey;

            try
            {
                fileName = CreateDbFile();
                string connectionString = (new SQLiteConnectionStringBuilder { DataSource = fileName }).ToString();
                Repository repository = new Repository(connectionString);

                Assert.That(repository, Is.Not.Null);

                // Tables
                tables = repository.AllTables();
                Assert.That(tables, Is.Not.Null);
                Assert.That(tables.Length, Is.EqualTo(2));

                // Table 1
                Assert.That(repository.TableExists("Table1"), Is.True);
                table = repository.GetTable("Table1");
                Assert.That(table, Is.Not.Null);
                Assert.That(table.SchemaName, Is.Null);
                Assert.That(table.Name, Is.EqualTo("Table1"));

                // Table 1 Columns
                columns = table.Columns();
                Assert.That(columns, Is.Not.Null);
                Assert.That(columns.Length, Is.EqualTo(6));

                // Table 1 Column Field1
                Assert.That(table.HasColumn("Field1"), Is.True);
                column = table.GetColumn("Field1");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table1"));
                Assert.That(column.Name, Is.EqualTo("Field1"));
                Assert.That(column.Position, Is.EqualTo(0));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.True);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 1 Column Field2
                Assert.That(table.HasColumn("Field2"), Is.True);
                column = table.GetColumn("Field2");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table1"));
                Assert.That(column.Name, Is.EqualTo("Field2"));
                Assert.That(column.Position, Is.EqualTo(1));
                Assert.That(column.DataType, Is.EqualTo("TEXT"));
                Assert.That(column.IsNullable, Is.False);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 1 Column Field3
                Assert.That(table.HasColumn("Field3"), Is.True);
                column = table.GetColumn("Field3");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table1"));
                Assert.That(column.Name, Is.EqualTo("Field3"));
                Assert.That(column.Position, Is.EqualTo(2));
                Assert.That(column.DataType, Is.EqualTo("REAL"));
                Assert.That(column.IsNullable, Is.True);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 1 Column Field4
                Assert.That(table.HasColumn("Field4"), Is.True);
                column = table.GetColumn("Field4");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table1"));
                Assert.That(column.Name, Is.EqualTo("Field4"));
                Assert.That(column.Position, Is.EqualTo(3));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.True);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);


                // Table 1 Column Field5
                Assert.That(table.HasColumn("Field5"), Is.True);
                column = table.GetColumn("Field5");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table1"));
                Assert.That(column.Name, Is.EqualTo("Field5"));
                Assert.That(column.Position, Is.EqualTo(4));
                Assert.That(column.DataType, Is.EqualTo("BLOB"));
                Assert.That(column.IsNullable, Is.True);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 1 Column Field6
                Assert.That(table.HasColumn("Field6"), Is.True);
                column = table.GetColumn("Field6");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table1"));
                Assert.That(column.Name, Is.EqualTo("Field6"));
                Assert.That(column.Position, Is.EqualTo(5));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.False);
                Assert.That(column.HasDefault, Is.True);
                Assert.That(column.Default, Is.EqualTo("10"));

                // Table 1 Primary Key
                primaryKey = table.PrimaryKey;
                Assert.That(primaryKey, Is.Not.Null);
                Assert.That(primaryKey.SchemaName, Is.Null);
                Assert.That(primaryKey.TableName, Is.EqualTo("Table1"));
                Assert.That(primaryKey.Name, Is.Null);
                columns = primaryKey.Columns();
                Assert.That(columns, Is.Not.Null);
                Assert.That(columns.Length, Is.EqualTo(1));
                Assert.That(columns[0], Is.EqualTo(table.GetColumn("Field1")));

                // Table 1 Indexes
                indexes = table.Indexes();
                Assert.That(indexes, Is.Not.Null);
                Assert.That(indexes.Length, Is.EqualTo(1));

                // Table 1 Index Unique
                index = indexes[0];
                Assert.That(index, Is.Not.Null);
                Assert.That(index.SchemaName, Is.Null);
                Assert.That(index.TableName, Is.EqualTo("Table1"));
                Assert.That(index.Name, Is.EqualTo("sqlite_autoindex_Table1_1")); // auto named
                Assert.That(index.IsUnique, Is.True);
                Assert.That(index.IsClustered, Is.Null);
                columnsForIndex = index.Columns();
                Assert.That(columnsForIndex, Is.Not.Null);
                Assert.That(columnsForIndex.Length, Is.EqualTo(1));
                columnForIndex = columnsForIndex[0];
                Assert.That(columnForIndex, Is.Not.Null);
                Assert.That(columnForIndex.Position, Is.EqualTo(0));
                Assert.That(columnForIndex.IsAsc, Is.Null);
                Assert.That(columnForIndex.Column, Is.EqualTo(table.GetColumn("Field2")));

                // Table 1 ForeignKeys
                foreignKeys = table.ForeignKeys();
                Assert.That(foreignKeys, Is.Not.Null);
                Assert.That(foreignKeys.Length, Is.EqualTo(0));

                // Table 2
                Assert.That(repository.TableExists("Table2"), Is.True);
                table = repository.GetTable("Table2");
                Assert.That(table, Is.Not.Null);
                Assert.That(table.SchemaName, Is.Null);
                Assert.That(table.Name, Is.EqualTo("Table2"));

                // Table 2 Columns
                columns = table.Columns();
                Assert.That(columns, Is.Not.Null);
                Assert.That(columns.Length, Is.EqualTo(5));

                // Table 2 Column Field1
                Assert.That(table.HasColumn("Field1"), Is.True);
                column = table.GetColumn("Field1");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table2"));
                Assert.That(column.Name, Is.EqualTo("Field1"));
                Assert.That(column.Position, Is.EqualTo(0));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.False);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 2 Column Field2
                Assert.That(table.HasColumn("Field2"), Is.True);
                column = table.GetColumn("Field2");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table2"));
                Assert.That(column.Name, Is.EqualTo("Field2"));
                Assert.That(column.Position, Is.EqualTo(1));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.False);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 2 Column Field3
                Assert.That(table.HasColumn("Field3"), Is.True);
                column = table.GetColumn("Field3");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table2"));
                Assert.That(column.Name, Is.EqualTo("Field3"));
                Assert.That(column.Position, Is.EqualTo(2));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.False);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 2 Column Field4
                Assert.That(table.HasColumn("Field4"), Is.True);
                column = table.GetColumn("Field4");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table2"));
                Assert.That(column.Name, Is.EqualTo("Field4"));
                Assert.That(column.Position, Is.EqualTo(3));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.False);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);

                // Table 2 Column Field5
                Assert.That(table.HasColumn("Field5"), Is.True);
                column = table.GetColumn("Field5");
                Assert.That(column, Is.Not.Null);
                Assert.That(column.SchemaName, Is.Null);
                Assert.That(column.TableName, Is.EqualTo("Table2"));
                Assert.That(column.Name, Is.EqualTo("Field5"));
                Assert.That(column.Position, Is.EqualTo(4));
                Assert.That(column.DataType, Is.EqualTo("INTEGER"));
                Assert.That(column.IsNullable, Is.False);
                Assert.That(column.HasDefault, Is.False);
                Assert.That(column.Default, Is.Null);


                // Table 2 Primary Key
                primaryKey = table.PrimaryKey;
                Assert.That(primaryKey, Is.Not.Null);
                Assert.That(primaryKey.SchemaName, Is.Null);
                Assert.That(primaryKey.TableName, Is.EqualTo("Table2"));
                Assert.That(primaryKey.Name, Is.Null);
                columns = primaryKey.Columns();
                Assert.That(columns, Is.Not.Null);
                Assert.That(columns.Length, Is.EqualTo(2));
                Assert.That(columns[0], Is.EqualTo(table.GetColumn("Field2")));
                Assert.That(columns[1], Is.EqualTo(table.GetColumn("Field3")));

                // Table 2 Indexes
                indexes = table.Indexes();
                Assert.That(indexes, Is.Not.Null);
                Assert.That(indexes.Length, Is.EqualTo(2));

                // Table 2 Index Unique
                index = indexes[0];
                Assert.That(index, Is.Not.Null);
                Assert.That(index.SchemaName, Is.Null);
                Assert.That(index.TableName, Is.EqualTo("Table2"));
                Assert.That(index.Name, Is.EqualTo("sqlite_autoindex_Table2_2")); // auto named
                Assert.That(index.IsUnique, Is.True);
                Assert.That(index.IsClustered, Is.Null);
                columnsForIndex = index.Columns();
                Assert.That(columnsForIndex, Is.Not.Null);
                Assert.That(columnsForIndex.Length, Is.EqualTo(2));
                columnForIndex = columnsForIndex[0];
                Assert.That(columnForIndex, Is.Not.Null);
                Assert.That(columnForIndex.Position, Is.EqualTo(0));
                Assert.That(columnForIndex.IsAsc, Is.Null);
                Assert.That(columnForIndex.Column, Is.EqualTo(table.GetColumn("Field4")));
                columnForIndex = columnsForIndex[1];
                Assert.That(columnForIndex, Is.Not.Null);
                Assert.That(columnForIndex.Position, Is.EqualTo(1));
                Assert.That(columnForIndex.IsAsc, Is.Null);
                Assert.That(columnForIndex.Column, Is.EqualTo(table.GetColumn("Field5")));

                // Table 2 Index PK
                index = indexes[1];
                Assert.That(index, Is.Not.Null);
                Assert.That(index.SchemaName, Is.Null);
                Assert.That(index.TableName, Is.EqualTo("Table2"));
                Assert.That(index.Name, Is.EqualTo("sqlite_autoindex_Table2_1")); // auto named
                Assert.That(index.IsUnique, Is.True);
                Assert.That(index.IsClustered, Is.Null);
                columnsForIndex = index.Columns();
                Assert.That(columnsForIndex, Is.Not.Null);
                Assert.That(columnsForIndex.Length, Is.EqualTo(2));
                columnForIndex = columnsForIndex[0];
                Assert.That(columnForIndex, Is.Not.Null);
                Assert.That(columnForIndex.Position, Is.EqualTo(0));
                Assert.That(columnForIndex.IsAsc, Is.Null);
                Assert.That(columnForIndex.Column, Is.EqualTo(table.GetColumn("Field2")));
                columnForIndex = columnsForIndex[1];
                Assert.That(columnForIndex, Is.Not.Null);
                Assert.That(columnForIndex.Position, Is.EqualTo(1));
                Assert.That(columnForIndex.IsAsc, Is.Null);
                Assert.That(columnForIndex.Column, Is.EqualTo(table.GetColumn("Field3")));

                // Table 2 ForeignKeys
                foreignKeys = table.ForeignKeys();
                Assert.That(foreignKeys, Is.Not.Null);
                Assert.That(foreignKeys.Length, Is.EqualTo(2));

                // Table 2 foreign Keys
                foreignKey = foreignKeys[0];
                Assert.That(index, Is.Not.Null);
                Assert.That(foreignKey.SourceSchemaName, Is.Null);
                Assert.That(foreignKey.SourceTableName, Is.EqualTo("Table2"));
                Assert.That(foreignKey.ReferenceSchemaName, Is.Null);
                Assert.That(foreignKey.ReferenceTableName, Is.EqualTo("Table1"));
                Assert.That(foreignKey.UpdateRule, Is.EqualTo("NO ACTION"));
                Assert.That(foreignKey.DeleteRule, Is.EqualTo("NO ACTION"));

                columnsForForeignKey = foreignKey.Columns();
                Assert.That(columnsForForeignKey, Is.Not.Null);
                Assert.That(columnsForForeignKey.Length, Is.EqualTo(2));
                columnForForeignKey = columnsForForeignKey[0];
                Assert.That(columnForForeignKey, Is.Not.Null);
                Assert.That(columnForForeignKey.Position, Is.EqualTo(0));
                Assert.That(columnForForeignKey.SourceColumn, Is.EqualTo(table.GetColumn("Field1")));
                Assert.That(columnForForeignKey.ReferenceColumn, Is.EqualTo(repository.GetTable("Table1").GetColumn("Field1")));
                columnForForeignKey = columnsForForeignKey[1];
                Assert.That(columnForForeignKey, Is.Not.Null);
                Assert.That(columnForForeignKey.Position, Is.EqualTo(1));
                Assert.That(columnForForeignKey.SourceColumn, Is.EqualTo(table.GetColumn("Field3")));
                Assert.That(columnForForeignKey.ReferenceColumn, Is.EqualTo(repository.GetTable("Table1").GetColumn("Field6")));
                foreignKey = foreignKeys[1];
                Assert.That(index, Is.Not.Null);
                Assert.That(foreignKey.SourceSchemaName, Is.Null);
                Assert.That(foreignKey.SourceTableName, Is.EqualTo("Table2"));
                Assert.That(foreignKey.ReferenceSchemaName, Is.Null);
                Assert.That(foreignKey.ReferenceTableName, Is.EqualTo("Table1"));
                Assert.That(foreignKey.UpdateRule, Is.EqualTo("NO ACTION"));
                Assert.That(foreignKey.DeleteRule, Is.EqualTo("NO ACTION"));

                columnsForForeignKey = foreignKey.Columns();
                Assert.That(columnsForForeignKey, Is.Not.Null);
                Assert.That(columnsForForeignKey.Length, Is.EqualTo(1));
                columnForForeignKey = columnsForForeignKey[0];
                Assert.That(columnForForeignKey, Is.Not.Null);
                Assert.That(columnForForeignKey.Position, Is.EqualTo(0));
                Assert.That(columnForForeignKey.SourceColumn, Is.EqualTo(table.GetColumn("Field1")));
                Assert.That(columnForForeignKey.ReferenceColumn, Is.EqualTo(repository.GetTable("Table1").GetColumn("Field1")));
            }
            finally
            {
                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }
    }
}