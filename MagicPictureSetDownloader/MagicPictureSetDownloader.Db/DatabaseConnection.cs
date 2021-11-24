namespace MagicPictureSetDownloader.Db
{
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;

    using Common.Database;

    using MagicPictureSetDownloader.DbGenerator;
    
    internal partial class DatabaseConnection
    {
        private string _connectionString;
        private IDbConnection _batchConnection;
        private IDbTransaction _batchTransaction;
        
        public DatabaseConnection()
        {
            IdentityRetriever.IdentityQuery = "SELECT last_insert_rowid()";
            GetConnectionString();
        }
        private void GetConnectionString()
        {
            string fileName = DatabaseGenerator.GetResourceName();
                
            // ReSharper disable AssignNullToNotNullAttribute
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            // ReSharper restore AssignNullToNotNullAttribute
            string connectionString = (new SQLiteConnectionStringBuilder { DataSource = filePath }).ToString();
            if (!File.Exists(filePath))
            {
                DatabaseGenerator.Generate();
            }

            DatabaseGenerator.VersionVerify(connectionString);
            _connectionString = connectionString;
        }
        private IDbConnection GetMagicConnectionInternal()
        {
            SQLiteConnection cnx = new SQLiteConnection(_connectionString);
            cnx.Open();
            return cnx;
        }
        private bool BatchModeActivated()
        {
            return _batchConnection != null;
        }

        public IDbConnection GetMagicConnection()
        {
            if (!BatchModeActivated())
            {
                return new ConnectionWrapper(GetMagicConnectionInternal(), false);
            }

            return new ConnectionWrapper(_batchConnection, true);
        }
        public void ActivateBatchMode()
        {
            if (BatchModeActivated())
            {
                throw new ApplicationDbException("BatchMode is already activated");
            }

            _batchConnection = GetMagicConnectionInternal();
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
