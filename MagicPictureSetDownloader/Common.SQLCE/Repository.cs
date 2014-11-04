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
        //TODO: Constrainsts, Keys, Views ...
        //TODO: case sensitive or not?
        private const string ColumnQuery = @"SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, AUTOINC_INCREMENT, AUTOINC_SEED, COLUMN_HASDEFAULT, COLUMN_DEFAULT, COLUMN_FLAGS, NUMERIC_SCALE, TABLE_NAME, AUTOINC_NEXT, TABLE_SCHEMA, ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_SCHEMA ASC, TABLE_NAME ASC, ORDINAL_POSITION ASC";
        private const string TableQuery = @" SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";

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

        public ITable GetTable(string name)
        {
            return GetTable(null, name);
        }
        public ITable GetTable(string schemaName, string name)
        {
            string tablekey = Table.TableKey(schemaName, name);

            Table table;
            _tables.TryGetValue(tablekey, out table);
            return table;
        }
        public bool RowExists(string schemaName, string name, string[] columnNames, object[] values)
        {
            ITable table = GetTable(schemaName, name);
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
                foreach (Table table in GetFromQuery(cnx, TableQuery, CreateTable))
                {
                    _tables.Add(table.ToString(), table);
                }
                foreach (Column column in GetFromQuery(cnx, ColumnQuery, CreateColumn))
                {
                    string tablekey = Table.TableKey(column.SchemaName, column.TableName);
                    Table table = _tables[tablekey];
                    table.AddColumn(column);
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

        private IEnumerable<T> GetFromQuery<T>(SqlCeConnection cnx, string query, Func<SqlCeDataReader, T> instanciateFromReader)
        {
            using (SqlCeCommand cmd = cnx.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    IList<T> ret = new List<T>();
                    while (reader.Read())
                    {
                        ret.Add(instanciateFromReader(reader));
                    }
                    return ret;
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
                       };
        }
        private Table CreateTable(IDataRecord dr)
        {
            return new Table
                       {
                           SchemaName = dr.GetStringOrDefault(0),
                           Name = dr.GetStringOrDefault(1),
                       };
        }
    }
}
