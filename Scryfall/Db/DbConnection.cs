namespace ScryfallTest.Db
{
    using Common.Database;
    using System.Data;
    using System.Data.SQLite;

    internal class DbConnection
    {
        private readonly string _connectionString;

        static DbConnection()
        {
            IdentityRetriever.IdentityQuery = "SELECT last_insert_rowid()";
        }

        public DbConnection(string dbPath = "MagicData.sqlite")
        {
            _connectionString = (new SQLiteConnectionStringBuilder { DataSource = dbPath }).ToString();
        }

        public IDbConnection GetConnection()
        {
            SQLiteConnection cnx = new SQLiteConnection(_connectionString);
            cnx.Open();
            return cnx;
        }
    }
}
