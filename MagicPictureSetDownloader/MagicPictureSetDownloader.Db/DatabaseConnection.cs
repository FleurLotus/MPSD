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
        private readonly IDictionary<DatabasebType, string> _connectionStrings = new Dictionary<DatabasebType, string>();
        private IDbConnection _batchConnection;
        private IDbTransaction _batchTransaction;
        
        public DatabaseConnection()
        {
            IdentityRetriever.IdentityQuery = "SELECT last_insert_rowid()";
            GetConnectionString(DatabasebType.Data);
            GetConnectionString(DatabasebType.Picture);
        }
        private void GetConnectionString(DatabasebType databaseType)
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
        private IDbConnection GetMagicConnectionInternal(DatabasebType databasebType)
        {
            SQLiteConnection cnx = new SQLiteConnection(_connectionStrings[databasebType]);
            cnx.Open();
            return cnx;
        }
        private bool BatchModeActivated()
        {
            return _batchConnection != null;
        }

        public IDbConnection GetMagicConnection(DatabasebType databasebType)
        {
            if (databasebType == DatabasebType.Picture || !BatchModeActivated())
            {
                return new ConnectionWrapper(GetMagicConnectionInternal(databasebType), false);
            }

            return new ConnectionWrapper(_batchConnection, true);
        }
        public void ActivateBatchMode()
        {
            if (BatchModeActivated())
            {
                throw new ApplicationDbException("BatchMode is already activated");
            }

            _batchConnection = GetMagicConnectionInternal(DatabasebType.Data);
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
