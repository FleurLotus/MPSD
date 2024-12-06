namespace Common.SQL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class PrimaryKey: IPrimaryKey, IComparable<IPrimaryKey>
    {
        private readonly SortedDictionary<int, IColumn> _columns = new SortedDictionary<int, IColumn>(); 

        public string Name { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public CaseSensitivity CaseSensitivity { get; internal set; }
        
        public IColumn[] Columns()
        {
            return _columns.Values.ToArray();
        }
        internal void AddColumn(int index, IColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (CaseSensitivity.Compare(SchemaName, column.SchemaName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong schema", nameof(column));
            }
            if (CaseSensitivity.Compare(TableName, column.TableName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong table", nameof(column));
            }

            _columns[index] = column;
        }

        public int CompareTo(IPrimaryKey other)
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
            return CaseSensitivity.ToKeyString($"{Table.TableKey(SchemaName, TableName, CaseSensitivity)}.{(Name ?? "*AUTO*")}", CaseSensitivity);
        }
    }
}
