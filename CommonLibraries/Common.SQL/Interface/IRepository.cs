namespace Common.SQL
{
    using System.Collections.Generic;

    public interface IRepository
    {
        ITable[] AllTables();

        ITable GetTable(string name);
        ITable GetTable(string schemaName, string name);
        bool TableExists(string name);
        bool TableExists(string schemaName, string name);
        
        bool ColumnExists(string tableName, string name);
        bool ColumnExists(string schemaName, string tableName, string name);
        
        bool RowExists(string schemaName, string tableName, string[] columnNames, object[] values);
        
        void Refresh();

        void ExecuteBatch(string sqlcommand);
        void ExecuteBatch(string sqlcommand, params string[] parameters);

        void ExecuteParametrizeCommand(string sqlcommand, params KeyValuePair<string, object>[] parameters);
        void ExecuteParametrizeCommandMulti(string sqlcommand, IEnumerable<KeyValuePair<string, object>[]> executionParameters);
    }
}