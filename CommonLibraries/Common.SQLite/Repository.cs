namespace Common.SQLite
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Linq;

    using Common.SQL;

    public class Repository : RepositoryBase
    {
        #region Query

        private const string ColumnQuery = @"PRAGMA table_info({0})";
        private const string IndexListQuery = @"PRAGMA index_list({0})";
        private const string IndexInfoQuery = @"PRAGMA index_info({0})";
        private const string TableQuery = @"SELECT name FROM sqlite_master WHERE type = 'table'";
        #endregion

        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;

            Refresh();
        }

        protected override DbConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public override sealed void Refresh()
        {
            using (DbConnection cnx = GetConnection())
            {
                cnx.Open();

                Tables.Clear();

                using (DbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    //Case Sensibility
                    cmd.CommandText = CaseSensitivityTestQuery;
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        IsCaseSensitive = new CaseSensitivity(reader.Read());
                    }

                    //Tables
                    cmd.CommandText = TableQuery;
                    using (DbDataReader reader = cmd.ExecuteReader())
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
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Column column = CreateColumn(reader, table);
                                table.AddColumn(column);

                                int pkpos = (int)reader.GetInt64OrDefault(5);
                                if (pkpos > 0)
                                {
                                    if (table.PrimaryKey == null)
                                        table.SetPrimaryKey(CreatePrimaryKey(table));

                                    PrimaryKey primaryKey = table.PrimaryKey as PrimaryKey;
                                    // ReSharper disable PossibleNullReferenceException
                                    primaryKey.AddColumn(pkpos, column);
                                    // ReSharper restore PossibleNullReferenceException
                                }
                            }
                        }
                    }

                    //Indexes
                    foreach (Table table in Tables.Values.Cast<Table>())
                    {
                        cmd.CommandText = string.Format(IndexListQuery, table.Name);
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Index index = CreateIndex(reader, table);
                                table.AddIndex(index);
                            }
                        }

                        foreach (Index index in table.Indexes().Cast<Index>())
                        {
                            cmd.CommandText = string.Format(IndexInfoQuery, index.Name);
                            using (DbDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    IColumn column = table.GetColumn(reader.GetStringOrDefault(2));
                                    index.AddColumn(new ColumnForIndex { Column = column, Position = (int)reader.GetInt64OrDefault(0) });
                                }
                            }
                        }
                    }
                    //ALERT: Foreign Key List TO BE CODED
                }
            }
        }

        private Column CreateColumn(IDataRecord dr, ITable table)
        {
            return new Column
                       {
                           Position = (int)dr.GetInt64OrDefault(0),
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
        private ForeignKey CreateForeignKey(IDataRecord dr)
        {
            return new ForeignKey
            {
                CaseSensitivity = IsCaseSensitive,
            };
        }
    }
}
