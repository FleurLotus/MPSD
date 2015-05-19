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
                throw new ArgumentNullException("column");
            
            _columns[index] = column;
        }

        public int CompareTo(IPrimaryKey other)
        {
            int comp = 0;
            if (string.IsNullOrEmpty(SchemaName))
            {
                if (!string.IsNullOrEmpty(other.SchemaName))
                    comp = -1;
            }
            else
            {
                comp = string.Compare(SchemaName, other.SchemaName, StringComparison.Ordinal);
            }

            if (comp == 0)
                comp = string.Compare(TableName, other.TableName, StringComparison.Ordinal);

            if (comp == 0)
                comp = string.Compare(CaseSensitivity.ToKeyString(Name), CaseSensitivity.ToKeyString(other.Name), StringComparison.Ordinal);

            return comp;
        }
        public override string ToString()
        {
            return string.Format("{0}.{1}", Table.TableKey(SchemaName, TableName, CaseSensitivity), Name??"*AUTO*");
        }
    }
}
