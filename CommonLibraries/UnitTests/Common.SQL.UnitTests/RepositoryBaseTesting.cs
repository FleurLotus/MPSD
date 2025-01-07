namespace Common.SQL.UnitTests
{
    using System.Data;

    internal class RepositoryBaseTesting : RepositoryBase
    {
        private readonly IDbConnection _connection;

        public RepositoryBaseTesting(IDbConnection connection)
        {
            IsCaseSensitive = new CaseSensitivity(true);

            Table table = new Table { Name = "TableName", SchemaName = "SchemaName", CaseSensitivity = IsCaseSensitive };
            table.AddColumn(new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName", Position = 0, CaseSensitivity = IsCaseSensitive });
            table.AddColumn(new Column { Name = "ColumnName2", SchemaName = "SchemaName", TableName = "TableName", Position = 1, CaseSensitivity = IsCaseSensitive });
            Tables.Add(table.ToString(), table);

            table = new Table { Name = "TableName2", SchemaName = "SchemaName", CaseSensitivity = IsCaseSensitive };
            table.AddColumn(new Column { Name = "ColumnName", SchemaName = "SchemaName", TableName = "TableName2", Position = 0, CaseSensitivity = IsCaseSensitive });
            table.AddColumn(new Column { Name = "ColumnName2", SchemaName = "SchemaName", TableName = "TableName2", Position = 1, CaseSensitivity = IsCaseSensitive });
            table.AddColumn(new Column { Name = "ColumnName3", SchemaName = "SchemaName", TableName = "TableName2", Position = 2, CaseSensitivity = IsCaseSensitive });
            Tables.Add(table.ToString(), table);

            table = new Table { Name = "TableName3", CaseSensitivity = IsCaseSensitive };
            table.AddColumn(new Column { Name = "ColumnName", TableName = "TableName3", Position = 0, CaseSensitivity = IsCaseSensitive });
            Tables.Add(table.ToString(), table);

            _connection = connection;
        }
        public override void Refresh()
        {
        }
        protected override IDbConnection GetConnection()
        {
            return _connection;
        }
    }
}
