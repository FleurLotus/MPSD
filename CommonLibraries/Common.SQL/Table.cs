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
            if (column == null)
                throw new ArgumentNullException("column");

            if (TableKey(column.SchemaName, column.TableName, column.CaseSensitivity) != ToString())
                throw new ArgumentException("Column doesn't belong to table", "column");

            _columns.Add(column);
            _columns.Sort();
        }
        public IColumn GetColumn(string name)
        {
            return _columns.FirstOrDefault(c => c.CaseSensitivity.ToKeyString(c.Name) == c.CaseSensitivity.ToKeyString(name));
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
            if (index == null)
                throw new ArgumentNullException("index");

            if (TableKey(index.SchemaName, index.TableName, index.CaseSensitivity) != ToString())
                throw new ArgumentException("Index doesn't belong to table", "index");

            _indexes.Add(index);
        }
        public IIndex GetIndex(string name)
        {
            return _indexes.FirstOrDefault(c => c.CaseSensitivity.ToKeyString(c.Name) == c.CaseSensitivity.ToKeyString(name));
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
            if (foreignKey == null)
                throw new ArgumentNullException("foreignKey");

            if (TableKey(foreignKey.SourceSchemaName, foreignKey.SourceTableName, foreignKey.CaseSensitivity) != ToString())
                throw new ArgumentException("ForeignKey doesn't belong to table", "foreignKey");

            _foreignKeys.Add(foreignKey);
        }
        public IForeignKey GetForeignKey(string name)
        {
            return _foreignKeys.FirstOrDefault(c => c.CaseSensitivity.ToKeyString(c.Name) == c.CaseSensitivity.ToKeyString(name));
        }
        public bool HasForeignKey(string name)
        {
            return GetForeignKey(name) != null;
        }
        
        internal void SetPrimaryKey(IPrimaryKey primaryKey)
        {
            if (primaryKey == null)
                throw new ArgumentNullException("primaryKey");

            if (TableKey(primaryKey.SchemaName, primaryKey.TableName, CaseSensitivity) != ToString())
                throw new ArgumentException("Primary Key doesn't belong to table", "primaryKey");

            if (PrimaryKey != null)
                throw new Exception("PrimaryKey is already set");

            PrimaryKey = primaryKey;
        }

        public override string ToString()
        {
            return TableKey(SchemaName, Name, CaseSensitivity);
        }
        public static string TableKey(string schemaName, string name, CaseSensitivity caseSensitivity)
        {
            return caseSensitivity.ToKeyString(string.IsNullOrEmpty(schemaName) ? name : string.Format("{0}.{1}", schemaName, name));
        }
    }
}
