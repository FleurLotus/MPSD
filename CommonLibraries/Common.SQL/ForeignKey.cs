namespace Common.SQL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ForeignKey : IForeignKey, IComparable<IForeignKey>
    {
        private readonly SortedDictionary<int, IColumnForForeignKey> _columns = new SortedDictionary<int, IColumnForForeignKey>();

        public string Name { get; set; }
        public string SourceTableName { get; set; }
        public string SourceSchemaName { get; set; }

        public string ReferenceTableName { get; set; }
        public string ReferenceSchemaName { get; set; }

        public string UpdateRule { get; set; }
        public string DeleteRule { get; set; }

        public CaseSensitivity CaseSensitivity { get; internal set; }

        public IColumnForForeignKey[] Columns()
        {
            return _columns.Values.ToArray();
        }

        internal void AddColumn(int position, IColumn source, IColumn reference)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(reference);

            if (CaseSensitivity.Compare(SourceSchemaName, source.SchemaName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong source schema", nameof(source));
            }
            if (CaseSensitivity.Compare(SourceTableName, source.TableName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong source table", nameof(source));
            }
            if (CaseSensitivity.Compare(ReferenceSchemaName, reference.SchemaName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong reference schema", nameof(reference));
            }
            if (CaseSensitivity.Compare(ReferenceTableName, reference.TableName, CaseSensitivity) != 0)
            {
                throw new ArgumentException("Wrong reference table", nameof(reference));
            }

            _columns[position] = new ColumnForForeignKey { SourceColumn = source, ReferenceColumn = reference, Position = position };
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
                comp = CaseSensitivity.Compare(SourceSchemaName, other.SourceSchemaName, CaseSensitivity);
            }

            if (comp == 0)
            {
                comp = CaseSensitivity.Compare(SourceTableName, other.SourceTableName, CaseSensitivity);
            }
            if (comp == 0)
            {
                comp = CaseSensitivity.Compare(Name, other.Name, CaseSensitivity);
            }
            return comp;
        }
        public override string ToString()
        {
            return CaseSensitivity.ToKeyString($"{Table.TableKey(SourceSchemaName, SourceTableName, CaseSensitivity)}.{Name}", CaseSensitivity);
        }
    }
}