namespace MagicPictureSetDownloader.Db
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;

    using Common.Database;

    using MagicPictureSetDownloader.DbGenerator;
    
    internal partial class DatabaseConnection
    {
        private readonly IDictionary<DatabaseType, string> _connectionStrings = new Dictionary<DatabaseType, string>();
        private IDbConnection _batchConnection;
        private IDbTransaction _batchTransaction;
        
        public DatabaseConnection()
        {
            IdentityRetriever.IdentityQuery = "SELECT last_insert_rowid()";
            GetConnectionString(DatabaseType.Data);
            GetConnectionString(DatabaseType.Picture);
        }
        private void GetConnectionString(DatabaseType databaseType)
        {
            string fileName = DatabaseGenerator.GetResourceName(databaseType);
                
            // ReSharper disable AssignNullToNotNullAttribute
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            // ReSharper restore AssignNullToNotNullAttribute
            string connectionString = (new SQLiteConnectionStringBuilder { DataSource = filePath }).ToString();
            if (!File.Exists(filePath))
            {
                DatabaseGenerator.Generate(databaseType);
            }

            DatabaseGenerator.VersionVerify(connectionString, databaseType);
            _connectionStrings[databaseType] = connectionString;
        }
        private IDbConnection GetMagicConnectionInternal(DatabaseType DatabaseType)
        {
            SQLiteConnection cnx = new SQLiteConnection(_connectionStrings[DatabaseType]);
            cnx.Open();
            return cnx;
        }
        private bool BatchModeActivated()
        {
            return _batchConnection != null;
        }

        public IDbConnection GetMagicConnection(DatabaseType DatabaseType)
        {
            if (DatabaseType == DatabaseType.Picture || !BatchModeActivated())
            {
                return new ConnectionWrapper(GetMagicConnectionInternal(DatabaseType), false);
            }

            return new ConnectionWrapper(_batchConnection, true);
        }
        public void ActivateBatchMode()
        {
            if (BatchModeActivated())
            {
                throw new ApplicationDbException("BatchMode is already activated");
            }

            _batchConnection = GetMagicConnectionInternal(DatabaseType.Data);
            _batchTransaction = _batchConnection.BeginTransaction();
        }
        public void DesactivateBatchMode()
        {
            if (!BatchModeActivated())
            {
                throw new ApplicationDbException("BatchMode is not activated");
            }
            _batchTransaction.Commit();
            _batchTransaction.Dispose();
            _batchTransaction = null;

            _batchConnection.Dispose();
            _batchConnection = null;
        }
    }
}
