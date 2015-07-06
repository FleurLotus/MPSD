﻿namespace Common.SQL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;

    public abstract class RepositoryBase : IRepository
    {
        protected const string CaseSensitivityTestQuery = @"SELECT 'AAAA' UNION SELECT 'aaaa'";

        protected CaseSensitivity IsCaseSensitive;
        protected readonly IDictionary<string, ITable> Tables = new Dictionary<string, ITable>();

        public ITable[] AllTables()
        {
            return Tables.Values.ToArray();
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
            string tablekey = Table.TableKey(schemaName, name, IsCaseSensitive);

            ITable table;
            Tables.TryGetValue(tablekey, out table);
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
            sb.AppendFormat("SELECT 1 FROM {0} WHERE ", table);

            using (DbConnection cnx = GetConnection())
            {
                cnx.Open();

                using (DbCommand cmd = cnx.CreateCommand())
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
                            DbParameter param = cmd.CreateParameter();
                            param.ParameterName = "@" + columnNames[i];
                            param.Value =  values[i];
                            cmd.Parameters.Add(param);
                        }
                    }

                    cmd.CommandText = sb.ToString();

                    object o = cmd.ExecuteScalar();
                    
                    return o != null && o != DBNull.Value;
                }
            }
        }
        public void ExecuteBatch(string sqlcommand)
        {
            string[] commands = sqlcommand.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            using (DbConnection cnx = GetConnection())
            {
                cnx.Open();
                using (DbCommand cmd = cnx.CreateCommand())
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

        
        public abstract void Refresh();
        protected abstract DbConnection GetConnection();
    }
}