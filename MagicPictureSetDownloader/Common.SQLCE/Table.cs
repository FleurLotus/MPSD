namespace Common.SQLCE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Table : ITable
    {
        private readonly List<Column> _columns = new List<Column>();

        public string SchemaName { get; set; }
        public string Name { get; set; }

        public IColumn[] Columns()
        {
            return _columns.Cast<IColumn>().ToArray();
        }
        internal void AddColumn(Column column)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            if (column.TableName != Name || column.SchemaName != SchemaName)
                throw new ArgumentException("Column doesn't belong to table", "column");

            _columns.Add(column);
            _columns.Sort();
        }
        public IColumn GetColumn(string name)
        {
            return _columns.FirstOrDefault(c => c.Name == name);
        }
        public bool HasColumn(string name)
        {
            return GetColumn(name) != null;
        }

        public override string ToString()
        {
            return TableKey( SchemaName, Name);
        }
        public static string TableKey(string schemaName, string name)
        {
            return string.IsNullOrEmpty(schemaName) ? name : string.Format("{0}.{1}", schemaName,name);
        }
    }
}
