namespace Common.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Linq;

    using Common.SQL;

    public class Repository : RepositoryBase
    {
        #region Query

        private const string ColumnQuery = @"PRAGMA table_info({0})";
        private const string IndexListQuery = @"PRAGMA index_list({0})";
        private const string IndexInfoQuery = @"PRAGMA index_info({0})";
        private const string ForeignKeyQuery = @"PRAGMA foreign_key_list({0})";
        private const string TableQuery = @"SELECT name FROM sqlite_master WHERE type = 'table'";
        #endregion

        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;

            Refresh();
        }

        protected override IDbConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public override sealed void Refresh()
        {
            using (IDbConnection cnx = GetConnection())
            {
                cnx.Open();

                Tables.Clear();

                using (IDbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    //Case Sensibility
                    cmd.CommandText = CaseSensitivityTestQuery;
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        IsCaseSensitive = new CaseSensitivity(reader.Read());
                    }

                    //Tables
                    cmd.CommandText = TableQuery;
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Table table = CreateTable(reader);
                            Tables.Add(table.ToString(), table);
                        }
                    }

                    //Columns & Primary Keys
                    foreach (Table table in Tables.Values.Cast<Table>())
                    {
                        cmd.CommandText = string.Format(ColumnQuery, table.Name);

                        PrimaryKey primaryKey = null;

                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Column column = CreateColumn(reader, table);
                                table.AddColumn(column);

                                int pkpos = (int) reader.GetInt64OrDefault(5);
                                if (pkpos > 0)
                                {
                                    primaryKey ??= CreatePrimaryKey(table);
                                    primaryKey.AddColumn(pkpos, column);
                                }
                            }

                            if (primaryKey != null)
                            {
                                table.SetPrimaryKey(primaryKey);
                            }
                        }
                    }

                    //Indexes
                    foreach (Table table in Tables.Values.Cast<Table>())
                    {
                        cmd.CommandText = string.Format(IndexListQuery, table.Name);
                        List<Index> indexes = new List<Index>();

                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                indexes.Add(CreateIndex(reader, table));
                            }
                        }

                        foreach (Index index in indexes)
                        {
                            cmd.CommandText = string.Format(IndexInfoQuery, index.Name);
                            using (IDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    IColumn column = table.GetColumn(reader.GetStringOrDefault(2));
                                    index.AddColumn((int) reader.GetInt64OrDefault(0), column);
                                }
                            }
                            table.AddIndex(index);
                        }
                    }

                    //Foreign Key
                    foreach (Table table in Tables.Values.Cast<Table>())
                    {
                        cmd.CommandText = string.Format(ForeignKeyQuery, table.Name);
                        IDictionary<int, ForeignKey> foreignKeys = new Dictionary<int, ForeignKey>();

                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = (int) reader.GetInt64OrDefault(0);

                                if (!foreignKeys.TryGetValue(id, out ForeignKey foreignKey))
                                {
                                    foreignKey = CreateForeignKey(reader, table);
                                    foreignKeys.Add(id, foreignKey);
                                }

                                int seq = (int) reader.GetInt64OrDefault(1);
                                string from = reader.GetStringOrDefault(3);
                                string to = reader.GetStringOrDefault(4);

                                foreignKey.AddColumn(seq, table.GetColumn(from), GetTable(foreignKey.ReferenceSchemaName, foreignKey.ReferenceTableName).GetColumn(to));
                            }
                        }

                        foreach (ForeignKey foreignKey in foreignKeys.Values)
                        {
                            table.AddForeignKey(foreignKey);
                        }
                    }
                }
            }
        }

        private Column CreateColumn(IDataRecord dr, ITable table)
        {
            return new Column
            {
                Position = (int) dr.GetInt64OrDefault(0),
                Name = dr.GetStringOrDefault(1),
                DataType = dr.GetStringOrDefault(2),
                IsNullable = dr.GetInt64OrDefault(3) == 0,
                HasDefault = dr.GetStringOrDefault(4) != null,
                Default = dr.GetStringOrDefault(4),
                TableName = table.Name,
                CaseSensitivity = IsCaseSensitive,
            };
        }
        private Table CreateTable(IDataRecord dr)
        {
            return new Table
            {
                Name = dr.GetStringOrDefault(0),
                CaseSensitivity = IsCaseSensitive,
            };
        }
        private PrimaryKey CreatePrimaryKey(ITable table)
        {
            return new PrimaryKey
            {
                TableName = table.Name,
                CaseSensitivity = IsCaseSensitive,
            };
        }
        private Index CreateIndex(IDataRecord dr, ITable table)
        {
            return new Index
            {
                Name = dr.GetStringOrDefault(1),
                IsUnique = dr.GetInt64OrDefault(2) == 1,
                TableName = table.Name,
                CaseSensitivity = IsCaseSensitive,
            };
        }
        private ForeignKey CreateForeignKey(IDataRecord dr, ITable table)
        {
            return new ForeignKey
            {
                SourceTableName = table.Name,
                ReferenceTableName = dr.GetStringOrDefault(2),
                UpdateRule = dr.GetStringOrDefault(5),
                DeleteRule = dr.GetStringOrDefault(6),
                Name = $"sqlite_autoforeignkey_{table.Name}_{(((int) dr.GetInt64OrDefault(0)) + 1)}",
                CaseSensitivity = IsCaseSensitive,
            };
        }
    }
}