namespace MagicPictureSetDownloader.Db
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.IO;
    using System.Reflection;

    using MagicPictureSetDownloader.DbGenerator;
    
    internal class DatabaseConnection
    {
        private readonly IDictionary<DatabasebType, string> _connectionStrings = new Dictionary<DatabasebType, string>();

        public DatabaseConnection()
            : this("MagicData.sdf", "MagicPicture.sdf")
        {
        }

        public DatabaseConnection(string fileName, string pictureFileName)
        {
            GetConnectionString(DatabasebType.Data, fileName);
            GetConnectionString(DatabasebType.Picture, pictureFileName);
        }
        private void GetConnectionString(DatabasebType databasebType, string fileName)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            string connectionString = "datasource=" + filePath + ";Max Database Size = 4091;";
            if (!File.Exists(filePath))
            {
                DatabaseGenerator.Generate(connectionString, databasebType);
            }

            DatabaseGenerator.VersionVerify(connectionString, databasebType);
            _connectionStrings[databasebType] = connectionString;
        }

        public DbConnection GetMagicConnection(DatabasebType databasebType)
        {
            SqlCeConnection cnx = new SqlCeConnection(_connectionStrings[databasebType]);
            cnx.Open();
            return cnx;
        }
    }
}
