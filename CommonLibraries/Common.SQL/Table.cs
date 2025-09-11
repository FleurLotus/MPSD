namespace Common.SQL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Table : ITable
    {
        private readonly List<Column> _columns = new List<Column>();
        private readonly List<Index> _indexes = new List<Index>();
        private readonly List<ForeignKey> _foreignKeys = new List<ForeignKey>();

        public string SchemaName { get; set; }
        public string Name { get; set; }
        public CaseSensitivity CaseSensitivity { get; internal set; }
        public IPrimaryKey PrimaryKey { get; private set; }

        public IColumn[] Columns()
        {
            return _columns.Cast<IColumn>().ToArray();
        }
        internal void AddColumn(Column column)
        {
            ArgumentNullException.ThrowIfNull(column);

            if (TableKey(column.SchemaName, column.TableName, column.CaseSensitivity) != ToString())
            {
                throw new ArgumentException("Column doesn't belong to table", nameof(column));
            }

            if (HasColumn(column.Name))
            {
                throw new ArgumentException("Column name already present", nameof(column));
            }

            if (_columns.Any(c => c.Position == column.Position))
            {
                throw new ArgumentException("Column position already present", nameof(column));
            }

            _columns.Add(column);
            _columns.Sort();
        }
        public IColumn GetColumn(string name)
        {
            return _columns.FirstOrDefault(c => CaseSensitivity.Compare(c.Name, name, CaseSensitivity) == 0);
        }
        public bool HasColumn(string name)
        {
            return GetColumn(name) != null;
        }

        public IIndex[] Indexes()
        {
            return _indexes.Cast<IIndex>().ToArray();
        }
        internal void AddIndex(Index index)
        {
            ArgumentNullException.ThrowIfNull(index);

            if (TableKey(index.SchemaName, index.TableName, index.CaseSensitivity) != ToString())
            {
                throw new ArgumentException("Index doesn't belong to table", nameof(index));
            }

            IColumnForIndex[] columns = index.Columns();
            if (columns.Length == 0)
            {
                throw new ArgumentException("Empty index", nameof(index));
            }
            if (columns.Any(c => !HasColumn(c.Column.Name)))
            {
                throw new ArgumentException("Invalid column in index", nameof(index));
            }
            if (_indexes.Any(i => CaseSensitivity.Compare(i.Name, index.Name, CaseSensitivity) == 0))
            {
                throw new ArgumentException("Index already present", nameof(index));
            }

            _indexes.Add(index);
        }
        public IIndex GetIndex(string name)
        {
            return _indexes.FirstOrDefault(c => CaseSensitivity.Compare(c.Name, name, CaseSensitivity) == 0);
        }
        public bool HasIndex(string name)
        {
            return GetIndex(name) != null;
        }

        public IForeignKey[] ForeignKeys()
        {
            return _foreignKeys.Cast<IForeignKey>().ToArray();
        }
        internal void AddForeignKey(ForeignKey foreignKey)
        {
            ArgumentNullException.ThrowIfNull(foreignKey);

            if (TableKey(foreignKey.SourceSchemaName, foreignKey.SourceTableName, foreignKey.CaseSensitivity) != ToString())
            {
                throw new ArgumentException("ForeignKey doesn't belong to table", nameof(foreignKey));
            }
            IColumnForForeignKey[] columns = foreignKey.Columns();
            if (columns.Length == 0)
            {
                throw new ArgumentException("Empty foreignKey", nameof(foreignKey));
            }
            if (columns.Any(c => !HasColumn(c.SourceColumn.Name)))
            {
                throw new ArgumentException("Invalid column in foreignKey", nameof(foreignKey));
            }
            if (_foreignKeys.Any(i => CaseSensitivity.Compare(i.Name, foreignKey.Name, CaseSensitivity) == 0))
            {
                throw new ArgumentException("ForeignKey already present", nameof(foreignKey));
            }

            _foreignKeys.Add(foreignKey);
        }
        public IForeignKey GetForeignKey(string name)
        {
            return _foreignKeys.FirstOrDefault(c => CaseSensitivity.Compare(c.Name, name, CaseSensitivity) == 0);
        }
        public bool HasForeignKey(string name)
        {
            return GetForeignKey(name) != null;
        }

        internal void SetPrimaryKey(IPrimaryKey primaryKey)
        {
            ArgumentNullException.ThrowIfNull(primaryKey);

            if (TableKey(primaryKey.SchemaName, primaryKey.TableName, CaseSensitivity) != ToString())
            {
                throw new ArgumentException("Primary Key doesn't belong to table", nameof(primaryKey));
            }
            IColumn[] columns = primaryKey.Columns();
            if (columns.Length == 0)
            {
                throw new ArgumentException("Empty primary key", nameof(primaryKey));
            }
            if (columns.Any(c => !HasColumn(c.Name)))
            {
                throw new ArgumentException("Invalid column in primary key", nameof(primaryKey));
            }
            if (PrimaryKey != null)
            {
                throw new Exception("PrimaryKey is already set");
            }

            PrimaryKey = primaryKey;
        }

        public override string ToString()
        {
            return TableKey(SchemaName, Name, CaseSensitivity);
        }
        public static string TableKey(string schemaName, string name, CaseSensitivity caseSensitivity)
        {
            return CaseSensitivity.ToKeyString(string.IsNullOrEmpty(schemaName) ? name : $"{schemaName}.{name}", caseSensitivity);
        }
    }
}