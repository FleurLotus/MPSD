namespace Common.SQL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Index : IIndex, IComparable<IIndex>
    {
        private readonly SortedDictionary<int, IColumnForIndex> _columns = new SortedDictionary<int, IColumnForIndex>();

        public string Name { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public bool IsUnique { get; set; }
        public bool? IsClustered { get; set; }
        public CaseSensitivity CaseSensitivity { get; internal set; }

        public IColumnForIndex[] Columns()
        {
            return _columns.Values.ToArray();
        }

        internal void AddColumn(int position, IColumn column, bool? isAsc = null)
        {
            ArgumentNullException.ThrowIfNull(column);

            if (CaseSensitivity.Compare(SchemaName, column.SchemaName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong schema", nameof(column));
            }
            if (CaseSensitivity.Compare(TableName, column.TableName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong table", nameof(column));
            }

            _columns[position] = new ColumnForIndex { Column = column, IsAsc = isAsc, Position = position };
        }

        public int CompareTo(IIndex other)
        {
            int comp = 0;
            if (string.IsNullOrEmpty(SchemaName))
            {
                if (!string.IsNullOrEmpty(other.SchemaName))
                {
                    comp = -1;
                }
            }
            else
            {
                comp = CaseSensitivity.Compare(SchemaName, other.SchemaName, CaseSensitivity);
            }

            if (comp == 0)
            {
                comp = CaseSensitivity.Compare(TableName, other.TableName, CaseSensitivity);
            }
            if (comp == 0)
            {
                comp = CaseSensitivity.Compare(Name, other.Name, CaseSensitivity);
            }
            return comp;
        }
        public override string ToString()
        {
            return CaseSensitivity.ToKeyString($"{Table.TableKey(SchemaName, TableName, CaseSensitivity)}.{Name}", CaseSensitivity);
        }
    }
}