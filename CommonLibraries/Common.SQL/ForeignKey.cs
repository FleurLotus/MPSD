namespace Common.SQL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ForeignKey : IForeignKey, IComparable<IForeignKey>
    {
        private readonly List<ColumnForForeignKey> _columns = new List<ColumnForForeignKey>();

        public string Name { get; set; }
        public string SourceTableName { get; set; }
        public string SourceSchemaName { get; set; }

        public string ReferenceName { get; set; }
        public string ReferenceTableName { get; set; }
        public string ReferenceSchemaName { get; set; }

        public string UpdateRule { get; set; }
        public string DeleteRule { get; set; }

        public CaseSensitivity CaseSensitivity { get; internal set; }
        
        public IColumnForForeignKey[] SourceColumns()
        {
            return _columns.Cast<IColumnForForeignKey>().ToArray();
        }

        internal void AddColumn(ColumnForForeignKey column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            _columns.Add(column);
            _columns.Sort();
        }

        public int CompareTo(IForeignKey other)
        {
            int comp = 0;
            if (string.IsNullOrEmpty(SourceSchemaName))
            {
                if (!string.IsNullOrEmpty(other.SourceSchemaName))
                {
                    comp = -1;
                }
            }
            else
            {
                comp = string.Compare(SourceSchemaName, other.SourceSchemaName, StringComparison.Ordinal);
            }

            if (comp == 0)
            {
                comp = string.Compare(SourceTableName, other.SourceTableName, StringComparison.Ordinal);
            }
            if (comp == 0)
            {
                comp = string.Compare(CaseSensitivity.ToKeyString(Name), CaseSensitivity.ToKeyString(other.Name), StringComparison.Ordinal);
            }
            return comp;
        }
        public override string ToString()
        {
            return string.Format("{0}.{1}", Table.TableKey(SourceSchemaName, SourceTableName, CaseSensitivity), Name);
        }

    }
}