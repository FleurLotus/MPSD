namespace MagicPictureSetDownloader.Db
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;

    using Common.Database;

    using MagicPictureSetDownloader.DbGenerator;
    
    internal class DatabaseConnection
    {
        private readonly IDictionary<DatabasebType, string> _connectionStrings = new Dictionary<DatabasebType, string>();

        public DatabaseConnection()
        {
            IdentityRetriever.IdentityQuery = "SELECT last_insert_rowid()";
            GetConnectionString(DatabasebType.Data);
            GetConnectionString(DatabasebType.Picture);
        }
        private void GetConnectionString(DatabasebType databasebType)
        {
            string fileName = DatabaseGenerator.GetResourceName(databasebType);
                
            // ReSharper disable AssignNullToNotNullAttribute
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            // ReSharper restore AssignNullToNotNullAttribute
            string connectionString = (new SQLiteConnectionStringBuilder { DataSource = filePath }).ToString();
            if (!File.Exists(filePath))
            {
                DatabaseGenerator.Generate(databasebType);
            }

            DatabaseGenerator.VersionVerify(connectionString, databasebType);
            _connectionStrings[databasebType] = connectionString;
        }

        public DbConnection GetMagicConnection(DatabasebType databasebType)
        {
            SQLiteConnection cnx = new SQLiteConnection(_connectionStrings[databasebType]);
            cnx.Open();
            return cnx;
        }
    }
}
