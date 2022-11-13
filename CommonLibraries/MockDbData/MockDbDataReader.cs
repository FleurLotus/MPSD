namespace MockDbData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    internal class MockDbDataReader : DbDataReader
    {
        private readonly IReadOnlyList<DataTable> _tables;
        private int _currentTableIndex;
        private int _currentRowIndex;
        private DataRow _currentRow;
        private DataTable _currentTable;
        private bool _closed;
        private readonly Dictionary<string, int> _columnMapping = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        public MockDbDataReader(MockDbResult mockDbResult)
        {
            if (mockDbResult == null)
            {
                throw new ArgumentNullException(nameof(mockDbResult));
            }
            _tables = mockDbResult.Tables;
            SetTable(0);
        }

        public MockDbDataReader()
        {
            SetTable(0);
        }

        public override object this[int ordinal]
        {
            get
            {
                CheckIsClosed();
                CheckData();
                return _currentRow[ordinal];
            }
        }
        public override object this[string name]
        {
            get
            {
                CheckIsClosed();
                CheckData();
                return _currentRow[name];
            }
        }
        public override int Depth
        {
            get
            {
                CheckIsClosed();
                return 0;
            }
        }
        public override int FieldCount
        {
            get
            {
                CheckIsClosed();
                return _currentTable?.Columns.Count ?? 0;
            }
        }
        public override bool HasRows
        {
            get
            {
                CheckIsClosed();
                return _currentTable?.Rows.Count > 0;
            }
        }
        public override bool IsClosed
        {
            get
            {
                return _closed;
            }
        }

        public override int RecordsAffected { get { return 0; } }

        public override void Close()
        {
            _closed = true;
        }
        public override bool GetBoolean(int ordinal)
        {
            return GetFieldValue<bool>(ordinal);
        }
        public override byte GetByte(int ordinal)
        {
            return GetFieldValue<byte>(ordinal);
        }
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            byte[] b = GetFieldValue<byte[]>(ordinal);
            long read = 0;

            for (long i = 0; i < length && dataOffset + i < b.Length; i++)
            {
                buffer[bufferOffset + i] = b[dataOffset + i];
                read++;
            }
            return read;
        }
        public override char GetChar(int ordinal)
        {
            return GetFieldValue<char>(ordinal);
        }
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            char[] c = GetFieldValue<char[]>(ordinal);
            long read = 0;

            for (long i = 0; i < length && dataOffset + i < c.Length; i++)
            {
                buffer[bufferOffset + i] = c[dataOffset + i];
                read++;
            }
            return read;
        }

        public override string GetDataTypeName(int ordinal)
        {
            return GetFieldType(ordinal)?.ToString();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return GetFieldValue<DateTime>(ordinal);
        }
        public override decimal GetDecimal(int ordinal)
        {
            return GetFieldValue<decimal>(ordinal);
        }
        public override double GetDouble(int ordinal)
        {
            return GetFieldValue<double>(ordinal);
        }
        public override IEnumerator GetEnumerator()
        {
            IList<object> ret = new List<object>();
            for (int i = 0; i < FieldCount; i++)
            {
                ret.Add(GetValue(i));
            }
            return ret.GetEnumerator();
        }
        public override Type GetFieldType(int ordinal)
        {
            CheckIsClosed();

            return _currentTable?.Columns[ordinal].DataType;
        }
        public override float GetFloat(int ordinal)
        {
            return GetFieldValue<float>(ordinal);
        }
        public override Guid GetGuid(int ordinal)
        {
            return GetFieldValue<Guid>(ordinal);
        }
        public override short GetInt16(int ordinal)
        {
            return GetFieldValue<short>(ordinal);
        }
        public override int GetInt32(int ordinal)
        {
            return GetFieldValue<int>(ordinal);
        }
        public override long GetInt64(int ordinal)
        {
            return GetFieldValue<long>(ordinal);
        }
        public override string GetName(int ordinal)
        {
            CheckIsClosed();
            return _currentTable?.Columns[ordinal].ColumnName;
        }
        public override int GetOrdinal(string name)
        {
            if (_columnMapping.TryGetValue(name, out int index))
            {
                return index;
            }

            return -1;
        }
        public override string GetString(int ordinal)
        {
            return GetFieldValue<string>(ordinal);
        }
        public override object GetValue(int ordinal)
        {
            return GetFieldValue<object>(ordinal);
        }
        public override int GetValues(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            CheckIsClosed();
            CheckData();

            int read = 0;
            for (int i = 0; i < values.Length && i < _columnMapping.Count; i++)
            {
                values[i] = GetValue(i);
                read++;
            }
            return read;
        }
        public override bool IsDBNull(int ordinal)
        {
            CheckIsClosed();
            CheckData();

            return _currentRow.IsNull(ordinal);
        }
        public override bool NextResult()
        {
            CheckIsClosed();

            if (_tables != null && _tables.Count > _currentTableIndex + 1)
            {
                SetTable(_currentTableIndex + 1);
                return true;
            }

            SetTable(int.MaxValue);
            return false;
        }
        public override bool Read()
        {
            CheckIsClosed();

            if (_currentTable == null)
            {
                _currentRow = null;
                return false;
            }

            if (_currentRowIndex + 1 >= _currentTable.Rows.Count)
            {
                _currentRow = null;
                return false;
            }

            _currentRowIndex++;
            _currentRow = _currentTable.Rows[_currentRowIndex];
            return true;
        }
        public override T GetFieldValue<T>(int ordinal)
        {
            CheckIsClosed();
            CheckData();

            return (T)(_currentRow[ordinal]);
        }
        private void CheckIsClosed()
        {
            if (IsClosed)
            {
                throw new InvalidOperationException("Could not access while reader is closed");
            }
        }
        private void SetTable(int tableIndex)
        {
            _columnMapping.Clear();
            if (_tables != null && _tables.Count > tableIndex)
            {
                _currentTableIndex = tableIndex;
                _currentTable = _tables[_currentTableIndex];

                for (int i = 0; i < _currentTable.Columns.Count; i++)
                {
                    _columnMapping.Add(_currentTable.Columns[i].ColumnName, i);
                }
            }
            else
            {
                _currentTableIndex = int.MaxValue;
                _currentTable = null;
            }
            _currentRowIndex = -1;
            _currentRow = null;
        }
        private void CheckData()
        {
            if (_currentRow == null)
            {
                throw new InvalidOperationException("no data");
            }
        }
    }
}
