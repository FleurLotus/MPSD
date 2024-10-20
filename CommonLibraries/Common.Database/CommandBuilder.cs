﻿namespace Common.Database
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Text;
    using Library.Enums;

    internal class CommandBuilder
    {
        private readonly TypeDbInfo _typeDbInfo;
        private string _selectQuery;
        private string _updateQuery;
        private string _deleteQuery;
        private string _insertQuery;
        private string[] _notKeycolumns;
        private string[] _canInsertColumns;
        
        public CommandBuilder(TypeDbInfo typeDbInfo)
        {
            _typeDbInfo = typeDbInfo;
            BuildQueries();
        }

        public void BuildSelectOneCommand(IDbCommand cmd, object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            CheckHasKey();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = _selectQuery;

            AppendWhereCriteriaCommand(cmd, input);
        }
        public void BuildSelectAllCommand(IDbCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = _selectQuery;
        }
        public void BuildDeleteAllCommand(IDbCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            CheckRestriction(Restriction.Delete);

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = _deleteQuery;
        }
        public void BuildUpdateOneCommand(IDbCommand cmd, object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            CheckRestriction(Restriction.Update);
            CheckHasKey();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = _updateQuery;
            
            foreach (string col in _notKeycolumns)
            {
                AddParameter(cmd, input, col);
            }

            AppendWhereCriteriaCommand(cmd, input);
        }
        public void BuildDeleteOneCommand(IDbCommand cmd, object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            CheckRestriction(Restriction.Delete);
            CheckHasKey();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = _deleteQuery;

            AppendWhereCriteriaCommand(cmd, input);
        }
        public void BuildInsertOneCommand(IDbCommand cmd, object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            CheckRestriction(Restriction.Insert);

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = _insertQuery;

            foreach (string col in _canInsertColumns)
            {
                AddParameter(cmd, input, col);
            }
        }
        public PropertyInfo GetIdKeyPropertyInfo()
        {
            return string.IsNullOrWhiteSpace(_typeDbInfo.Identity) ? null : _typeDbInfo.Columns[_typeDbInfo.Identity];
        }
        public IDictionary<int, PropertyInfo> GenerateReaderMap(IDataRecord reader)
        {
            IDictionary<int, PropertyInfo> map = new Dictionary<int, PropertyInfo>();
            foreach (KeyValuePair<string, PropertyInfo> kv in _typeDbInfo.Columns)
            {
                map.Add(reader.GetOrdinal(kv.Key), kv.Value);
            }
            return map;
        }

        private void BuildQueries()
        {
            StringBuilder sbSelect = new StringBuilder();
            sbSelect.Append("SELECT ");

            StringBuilder sbUpdate = new StringBuilder();
            sbUpdate.Append("UPDATE [");
            sbUpdate.Append(_typeDbInfo.TableName);
            sbUpdate.Append("] SET ");

            StringBuilder sbInsert = new StringBuilder();
            StringBuilder sbInsertValues = new StringBuilder();
            sbInsert.Append("INSERT INTO [");
            sbInsert.Append(_typeDbInfo.TableName);
            sbInsert.Append("] (");

            StringBuilder sbDelete = new StringBuilder();
            sbDelete.Append("DELETE FROM [");
            sbDelete.Append(_typeDbInfo.TableName);
            sbDelete.Append(']');

            string[] allColums = _typeDbInfo.Columns.Keys.ToArray();
            _notKeycolumns = _typeDbInfo.Columns.Keys.Where(s => !_typeDbInfo.Keys.Contains(s)).ToArray();
            _canInsertColumns = _typeDbInfo.Columns.Keys.Where(s => s != _typeDbInfo.Identity).ToArray();

            for (int i = 0; i < allColums.Length; i++)
            {
                if (i != 0)
                {
                    sbSelect.Append(", ");
                }

                sbSelect.Append("[" + allColums[i] + "]");
            }

            sbSelect.Append(" FROM [");
            sbSelect.Append(_typeDbInfo.TableName);
            sbSelect.Append(']');

            _selectQuery = sbSelect.ToString();

            for (int i = 0; i < _notKeycolumns.Length; i++)
            {
                string col = _notKeycolumns[i];
                if (i != 0)
                {
                    sbUpdate.Append(", ");
                }
                sbUpdate.AppendFormat("[{0}] = @{0}", col);
            }
            _updateQuery = sbUpdate.ToString();

            for (int i = 0; i < _canInsertColumns.Length; i++)
            {
                string col = _canInsertColumns[i];
                if (i != 0)
                {
                    sbInsert.Append(", ");
                    sbInsertValues.Append(", ");
                }

                sbInsert.Append("[" + col + "]");
                sbInsertValues.AppendFormat("@{0}", col);
            }
            sbInsert.AppendFormat(") VALUES ({0})", sbInsertValues);

            _insertQuery = sbInsert.ToString();

            _deleteQuery = sbDelete.ToString();
        }
        private void AppendWhereCriteriaCommand(IDbCommand cmd, object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            StringBuilder sb = new StringBuilder(cmd.CommandText);

            sb.Append(" WHERE ");
            for (int i = 0; i < _typeDbInfo.Keys.Count; i++)
            {
                string key = _typeDbInfo.Keys[i];
                if (i != 0)
                {
                    sb.Append(" AND ");
                }

                sb.AppendFormat("([{0}] = @{0})", key);
                AddParameter(cmd, input, key);
            }

            cmd.CommandText = sb.ToString();
        }
        private void AddParameter(IDbCommand cmd, object input, string col)
        {
            IDbDataParameter parameter = cmd.CreateParameter();
            PropertyInfo pi = _typeDbInfo.Columns[col];
            parameter.ParameterName = $"@{col}";
            object value = pi.GetValue(input, null);
            parameter.Value = value ?? DBNull.Value;
            DbType? dbtype = pi.PropertyType.ToDbType();
            //If not type found let the underlying provider try is one job
            if (dbtype.HasValue)
            {
                parameter.DbType = dbtype.Value;
            }

            cmd.Parameters.Add(parameter);
        }

        private void CheckHasKey()
        {
            if (_typeDbInfo.Keys.Count == 0)
            {
                throw new AttributedTypeException(null, "DbKeyColumnAttribute must be declared one and one time for the type when using OneCommand");
            }
        }

        private void CheckRestriction(Restriction restriction)
        {
            if (Matcher<Restriction>.IncludeValue(_typeDbInfo.Restriction, restriction))
            {
                throw new RestrictedDmlException(_typeDbInfo.TableName, restriction);
            }
        }
    }
}
