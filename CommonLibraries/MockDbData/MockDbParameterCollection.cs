namespace MockDbData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    public class MockDbParameterCollection : DbParameterCollection
    {
        private readonly List<MockDbParameter> _parameterList;

        internal MockDbParameterCollection()
        {
            _parameterList = new List<MockDbParameter>();
        }

        public override int Count { get { return _parameterList.Count; } }
        public override bool IsFixedSize { get { return false; } }
        public override bool IsReadOnly { get { return false; } }
        public override bool IsSynchronized { get { return false; } }
        public override object SyncRoot { get { return null; } }

        public new MockDbParameter this[string parameterName]
        {
            get { return (MockDbParameter)GetParameter(parameterName); }
            set { SetParameter(parameterName, value); }
        }
        public new MockDbParameter this[int index]
        {
            get { return (MockDbParameter)GetParameter(index); }
            set { SetParameter(index, value); }
        }

        public MockDbParameter Add(string parameterName, DbType parameterType)
        {
            MockDbParameter sQLiteParameter = new MockDbParameter(parameterName, parameterType);
            Add(sQLiteParameter);
            return sQLiteParameter;
        }
        public int Add(MockDbParameter parameter)
        {
            int count = -1;
            if (!string.IsNullOrEmpty(parameter.ParameterName))
            {
                count = IndexOf(parameter.ParameterName);
            }
            if (count == -1)
            {
                count = _parameterList.Count;
                _parameterList.Add(parameter);
            }
            SetParameter(count, parameter);
            return count;
        }
        public override int Add(object value)
        {
            return Add((MockDbParameter)value);
        }
        public void AddRange(MockDbParameter[] values)
        {
            int length = values.Length;
            for (int i = 0; i < length; i++)
            {
                Add(values[i]);
            }
        }
        public override void AddRange(Array values)
        {
            int length = values.Length;
            for (int i = 0; i < length; i++)
            {
                Add((MockDbParameter)values.GetValue(i));
            }
        }
        public MockDbParameter AddWithValue(string parameterName, object value)
        {
            MockDbParameter sQLiteParameter = new MockDbParameter(parameterName, value);
            Add(sQLiteParameter);
            return sQLiteParameter;
        }
        public override void Clear()
        {
            _parameterList.Clear();
        }
        public override bool Contains(string parameterName)
        {
            return IndexOf(parameterName) != -1;
        }
        public override bool Contains(object value)
        {
            return _parameterList.Contains((MockDbParameter)value);
        }
        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        public override IEnumerator GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }
        protected override DbParameter GetParameter(string parameterName)
        {
            return GetParameter(IndexOf(parameterName));
        }
        protected override DbParameter GetParameter(int index)
        {
            return _parameterList[index];
        }
        public override int IndexOf(string parameterName)
        {
            int count = _parameterList.Count;
            for (int i = 0; i < count; i++)
            {
                if (string.Compare(parameterName, _parameterList[i].ParameterName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public override int IndexOf(object value)
        {
            return _parameterList.IndexOf((MockDbParameter)value);
        }
        public override void Insert(int index, object value)
        {
            _parameterList.Insert(index, (MockDbParameter)value);
        }
        public override void Remove(object value)
        {
            _parameterList.Remove((MockDbParameter)value);
        }
        public override void RemoveAt(string parameterName)
        {
            RemoveAt(IndexOf(parameterName));
        }
        public override void RemoveAt(int index)
        {
            _parameterList.RemoveAt(index);
        }
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            SetParameter(IndexOf(parameterName), value);
        }
        protected override void SetParameter(int index, DbParameter value)
        {
            _parameterList[index] = (MockDbParameter) value;
        }
    }
}