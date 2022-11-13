namespace MockDbData
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    public class MockDbResult
    {
        public MockDbResult(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            Tables = new List<DataTable> { table }.AsReadOnly();
        }
        public MockDbResult(DataTable[] tables)
        {
            if (tables == null)
            {
                throw new ArgumentNullException(nameof(tables));
            }
            if (tables.Any(t => t == null))
            {
                throw new ArgumentNullException(nameof(tables));
            }
            Tables = new List<DataTable>(tables).AsReadOnly();
        }
        public IReadOnlyList<DataTable> Tables { get; }
    }
}