namespace Common.SQL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Index : IIndex, IComparable<IIndex>
    {
        private readonly List<ColumnForIndex> _columns = new List<ColumnForIndex>();

        public string Name { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public bool IsUnique { get; set; }
        public bool? IsClustered { get; set; }
        public CaseSensitivity CaseSensitivity { get; internal set; }

        public IColumnForIndex[] Columns()
        {
            return _columns.Cast<IColumnForIndex>().ToArray();
        }

        internal void AddColumn(ColumnForIndex column)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            _columns.Add(column);
            _columns.Sort();
        }

        public int CompareTo(IIndex other)
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
            return string.Format("{0}.{1}", Table.TableKey(SchemaName, TableName, CaseSensitivity), Name);
        }
    }
}