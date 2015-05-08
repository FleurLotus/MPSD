namespace Common.SQLCE
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Linq;
    using System.Text;

    using Common.Database;

    public class Repository
    {
        #region Query

        private const string ColumnQuery = @"
SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, 
       AUTOINC_INCREMENT, AUTOINC_SEED, COLUMN_HASDEFAULT, COLUMN_DEFAULT, COLUMN_FLAGS, 
       NUMERIC_SCALE, TABLE_NAME, AUTOINC_NEXT, TABLE_SCHEMA, ORDINAL_POSITION 
FROM INFORMATION_SCHEMA.COLUMNS 
ORDER BY TABLE_SCHEMA ASC, TABLE_NAME ASC, ORDINAL_POSITION ASC
";
        private const string TableQuery = @"
SELECT TABLE_SCHEMA, TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES
";
        private const string IndexQuery = @"
SELECT INDEX_NAME, TABLE_SCHEMA, TABLE_NAME, [UNIQUE], [CLUSTERED], ORDINAL_POSITION, COLUMN_NAME, COLLATION AS SORT_ORDER 
FROM INFORMATION_SCHEMA.INDEXES 
WHERE (PRIMARY_KEY = 0)  
ORDER BY TABLE_SCHEMA, TABLE_NAME, INDEX_NAME, ORDINAL_POSITION
";
        private const string ForeignKeyQuery = @"
SELECT SRC.CONSTRAINT_SCHEMA AS FK_CONSTRAINT_SCHEMA, SRC.TABLE_NAME AS FK_TABLE_NAME, SRC.CONSTRAINT_NAME AS FK_CONSTRAINT_NAME, SRC.COLUMN_NAME AS FK_COLUMN_NAME,
	   REF.CONSTRAINT_SCHEMA AS UQ_CONSTRAINT_SCHEMA, REF.TABLE_NAME AS UQ_TABLE_NAME, REF.CONSTRAINT_NAME AS UQ_CONSTRAINT_NAME, REF.COLUMN_NAME AS UQ_COLUMN_NAME, 
	   RC.UPDATE_RULE, RC.DELETE_RULE, REF.ORDINAL_POSITION AS UQ_ORDINAL_POSITION, SRC.ORDINAL_POSITION AS FK_ORDINAL_POSITION 
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC 
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE SRC ON SRC.CONSTRAINT_NAME = RC.CONSTRAINT_NAME AND 
											    CASE WHEN SRC.CONSTRAINT_SCHEMA IS NULL THEN '' ELSE SRC.CONSTRAINT_SCHEMA END = CASE WHEN RC.CONSTRAINT_SCHEMA IS NULL THEN '' ELSE RC.CONSTRAINT_SCHEMA END 
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE REF ON REF.CONSTRAINT_NAME =  RC.UNIQUE_CONSTRAINT_NAME AND 
												CASE WHEN REF.CONSTRAINT_SCHEMA IS NULL THEN '' ELSE REF.CONSTRAINT_SCHEMA END = CASE WHEN RC.UNIQUE_CONSTRAINT_SCHEMA IS NULL THEN '' ELSE RC.UNIQUE_CONSTRAINT_SCHEMA END AND
												REF.ORDINAL_POSITION = SRC.ORDINAL_POSITION AND REF.TABLE_NAME = RC.UNIQUE_CONSTRAINT_TABLE_NAME 
ORDER BY FK_CONSTRAINT_SCHEMA, FK_TABLE_NAME, FK_CONSTRAINT_NAME, FK_ORDINAL_POSITION
";
        private const string PrimaryKeyQuery = @"
SELECT c.CONSTRAINT_NAME, u.TABLE_SCHEMA, u.TABLE_NAME, u.COLUMN_NAME, u.ORDINAL_POSITION
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS c 
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE u ON c.CONSTRAINT_NAME = u.CONSTRAINT_NAME 
WHERE c.CONSTRAINT_TYPE = 'PRIMARY KEY'
ORDER BY c.CONSTRAINT_NAME, u.ORDINAL_POSITION
";
        private const string CaseSensitivityTestQuery = @"SELECT 'AAAA' UNION SELECT 'aaaa'";

        #endregion
        
        private CaseSensitivity _isCaseSensitive;
        private readonly string _connectionString;
        private readonly IDictionary<string, Table> _tables;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
            _tables = new Dictionary<string, Table>();

            Refresh();
        }

        public ITable[] AllTables()
        {
            return _tables.Values.Cast<ITable>().ToArray();
        }
        public bool TableExists(string name)
        {
            return GetTable(name) != null;
        }
        public bool TableExists(string schemaName, string name)
        {
            return GetTable(schemaName, name) != null;
        }

        public bool ColumnExists(string tableName, string name)
        {
            ITable table = GetTable(tableName);
            return table != null && table.HasColumn(name);
        }
        public bool ColumnExists(string schemaName, string tableName, string name)
        {
            ITable table = GetTable(schemaName, tableName);
            return table != null && table.HasColumn(name);
        }
        
        public ITable GetTable(string name)
        {
            return GetTable(null, name);
        }
        public ITable GetTable(string schemaName, string name)
        {
            string tablekey = Table.TableKey(schemaName, name, _isCaseSensitive);

            Table table;
            _tables.TryGetValue(tablekey, out table);
            return table;
        }
        public bool RowExists(string schemaName, string tableName, string[] columnNames, object[] values)
        {
            ITable table = GetTable(schemaName, tableName);
            if (table == null)
                throw new Exception("Unknown table");

            if (columnNames == null || columnNames.Length == 0)
                throw new ArgumentException("columnNames");

            if (values == null || values.Length == 0)
                throw new ArgumentException("values");

            if (values.Length != columnNames.Length)
                throw new Exception("columnNames and values must have the same length");

            if (columnNames.Any(columnName => !table.HasColumn(columnName)))
                throw new Exception("Unknown column");


            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select 1 FROM {0} WHERE ", table);


            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();

                using (SqlCeCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;


                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        if (i != 0)
                            sb.Append(" AND ");

                        if (values[i] == null)
                        {
                            sb.AppendFormat("([{0}] IS NULL)", columnNames[i]);
                        }
                        else
                        {
                            sb.AppendFormat("([{0}] = @{0})", columnNames[i]);
                            cmd.Parameters.AddWithValue("@" + columnNames[i], values[i]);
                        }
                    }

                    cmd.CommandText = sb.ToString();

                    object o = cmd.ExecuteScalar();
                    
                    return o != null && o != DBNull.Value;
                }
            }
        }

        public void Refresh()
        {
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();

                _tables.Clear();

                using (SqlCeCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    //Case Sensibility
                    cmd.CommandText = CaseSensitivityTestQuery;
                    using (SqlCeDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        _isCaseSensitive = new CaseSensitivity(reader.Read());
                    }
                    
                    //Tables
                    cmd.CommandText = TableQuery;
                    using (SqlCeDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Table table = CreateTable(reader);
                            _tables.Add(table.ToString(), table);
                        }
                    }

                    //Columns
                    cmd.CommandText = ColumnQuery;
                    using (SqlCeDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Column column = CreateColumn(reader);
                            string tablekey = Table.TableKey(column.SchemaName, column.TableName, _isCaseSensitive);
                            Table table = _tables[tablekey];
                            table.AddColumn(column);
                        }
                    }

                    //Primary Keys
                    cmd.CommandText = PrimaryKeyQuery;
                    using (SqlCeDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PrimaryKey primaryKey = CreatePrimaryKey(reader);
                            string tablekey = Table.TableKey(primaryKey.SchemaName, primaryKey.TableName, _isCaseSensitive);
                            Table table = _tables[tablekey];
                            
                            if (table.PrimaryKey == null)
                                table.SetPrimaryKey(primaryKey);

                            primaryKey = table.PrimaryKey as PrimaryKey;
                            // ReSharper disable PossibleNullReferenceException
                            primaryKey.AddColumn(reader.GetInt32OrDefault(4), table.GetColumn(reader.GetStringOrDefault(3)));
                            // ReSharper restore PossibleNullReferenceException
                        }
                    }

                    //Indexes
                    cmd.CommandText = IndexQuery;
                    using (SqlCeDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Index index = CreateIndex(reader);
                            string tablekey = Table.TableKey(index.SchemaName, index.TableName, _isCaseSensitive);
                            Table table = _tables[tablekey];

                            if (table.HasIndex(index.Name))
                            {
                                index = table.GetIndex(index.Name) as Index;
                            }
                            else
                            {
                                table.AddIndex(index);
                            }

                            IColumn column = table.GetColumn(reader.GetStringOrDefault(6));
                            // ReSharper disable PossibleNullReferenceException
                            index.AddColumn(new ColumnForIndex { Column = column, IsAsc = (reader.GetInt16OrDefault(7) == 1), Position = reader.GetInt32OrDefault(5) });
                            // ReSharper restore PossibleNullReferenceException
                        }
                    }

                    //Foreign Key
                    cmd.CommandText = ForeignKeyQuery;
                    using (SqlCeDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ForeignKey foreignKey = CreateForeignKey(reader);
                            string sourceTablekey = Table.TableKey(foreignKey.SourceSchemaName, foreignKey.SourceTableName, _isCaseSensitive);
                            string referenceTablekey = Table.TableKey(foreignKey.ReferenceSchemaName, foreignKey.ReferenceTableName, _isCaseSensitive);

                            Table sourceTable = _tables[sourceTablekey];
                            Table referenceTable = _tables[referenceTablekey];
                            
                            if (sourceTable.HasForeignKey(foreignKey.Name))
                            {
                                foreignKey = sourceTable.GetForeignKey(foreignKey.Name) as ForeignKey;
                            }
                            else
                            {
                                sourceTable.AddForeignKey(foreignKey);
                            }

                            IColumn sourceColumn = sourceTable.GetColumn(reader.GetStringOrDefault(3));
                            IColumn referenceColumn = referenceTable.GetColumn(reader.GetStringOrDefault(7));
                            // ReSharper disable PossibleNullReferenceException
                            foreignKey.AddColumn(new ColumnForForeignKey{SourceColumn = sourceColumn, ReferenceColumn =  referenceColumn, SourcePosition = reader.GetInt32OrDefault(10), ReferencePosition = reader.GetInt32(11)});
                            // ReSharper restore PossibleNullReferenceException
                        }
                    }
                }
            }
        }
        public void ExecuteBatch(string sqlcommand)
        {
            string[] commands = sqlcommand.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                using (SqlCeCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    foreach (string command in commands)
                    {
                        string trimcommand = command.TrimEnd(new[] { '\r', '\n' });
                        if (!string.IsNullOrWhiteSpace(trimcommand))
                        {
                            cmd.CommandText = trimcommand;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        
        private Column CreateColumn(IDataRecord dr)
        {
            return new Column
                       {
                           Name = dr.GetStringOrDefault(0),
                           IsNullable = dr.GetStringOrDefault(1) == "YES",
                           DataType = dr.GetStringOrDefault(2),
                           CharacterMaxLength = dr.GetInt32OrDefault(3),
                           NumericPrecision = dr.GetInt16OrDefault(4),
                           AutoIncrementBy = dr.GetInt64OrDefault(5),
                           AutoIncrementSeed = dr.GetInt64OrDefault(6),
                           HasDefault = dr.GetBoolOrDefault(7),
                           Default = dr.GetStringOrDefault(8),
                           RowGuidCol = !dr.IsDBNull(9) && (dr.GetInt32(9) == 378 || dr.GetInt32(9) == 282),
                           NumericScale = dr.GetInt16OrDefault(10),
                           TableName = dr.GetStringOrDefault(11),
                           AutoIncrementNext = dr.GetInt64OrDefault(12),
                           SchemaName = dr.GetStringOrDefault(13),
                           Position = dr.GetInt32OrDefault(14),
                           CaseSensitivity = _isCaseSensitive,
                       };
        }
        private Table CreateTable(IDataRecord dr)
        {
            return new Table
                       {
                           SchemaName = dr.GetStringOrDefault(0),
                           Name = dr.GetStringOrDefault(1),
                           CaseSensitivity = _isCaseSensitive,
                       };
        }
        private PrimaryKey CreatePrimaryKey(IDataRecord dr)
        {
            return new PrimaryKey
            {
                Name = dr.GetStringOrDefault(0),
                SchemaName = dr.GetStringOrDefault(1),
                TableName = dr.GetStringOrDefault(2),
                CaseSensitivity = _isCaseSensitive,
            };
        }
        private Index CreateIndex(IDataRecord dr)
        {
            return new Index
            {
                Name = dr.GetStringOrDefault(0),
                SchemaName = dr.GetStringOrDefault(1),
                TableName = dr.GetStringOrDefault(2),
                IsUnique = dr.GetBoolOrDefault(3),
                IsClustered = dr.GetBoolOrDefault(4),
                CaseSensitivity = _isCaseSensitive,
            };
        }
        private ForeignKey CreateForeignKey(IDataRecord dr)
        {
            return new ForeignKey
            {
                SourceSchemaName = dr.GetStringOrDefault(0),
                SourceTableName = dr.GetStringOrDefault(1),
                Name = dr.GetStringOrDefault(2),

                ReferenceSchemaName = dr.GetStringOrDefault(4),
                ReferenceTableName = dr.GetStringOrDefault(5),
                ReferenceName = dr.GetStringOrDefault(6),

                UpdateRule = dr.GetStringOrDefault(8),
                DeleteRule = dr.GetStringOrDefault(9),
                
                CaseSensitivity = _isCaseSensitive,
            };
        }
    }
}
