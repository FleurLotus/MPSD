namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Reflection;

    using Common.SQL;
    using Common.SQLite;

    public class Upgrader
    {
        private const string VersionQuery = "SELECT Major FROM Version";
        private const string UpdateVersionQuery = "UPDATE Version Set Major = ";
        private readonly string _connectionString;
        private readonly DatabasebType _data;
        private static readonly Version _applicationVersion;

        static Upgrader()
        {
            Assembly entryAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = entryAssembly.GetName();
            _applicationVersion = assemblyName.Version;
        }
        internal Upgrader(string connectionString, DatabasebType data)
        {
            _connectionString = connectionString;
            _data = data;
        }

        internal void Upgrade()
        {
            int version;
            using (SQLiteConnection cnx = new SQLiteConnection(_connectionString))
            {
                cnx.Open();
                using (DbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = VersionQuery;

                    version = (int)(long)(cmd.ExecuteScalar());
                }
            }
            try
            {
                ExecuteUpgradeCommands(version);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while upgrading database", ex);
            }
        }

        private void ExecuteUpgradeCommands(int dbVersion)
        {
            if (_applicationVersion.Major < dbVersion)
                throw new Exception("Db is newer that application");

//We redo the change of current version because of minor version upgrade          
/*
            if (_applicationVersion.Major == dbVersion)
                return;
*/

            if (dbVersion < 6)
                throw new Exception("You have udpated the version!!! There is no released version with this db version");

            Repository repo = new Repository(_connectionString);
            switch (_data)
            {
                case DatabasebType.Data:
                    UpgradeData(dbVersion, repo);
                    break;
                case DatabasebType.Picture:
                    break;
            }
            
#if !DEBUG 
            //No update of Version in debug
			if (_applicationVersion.Major != dbVersion)
				repo.ExecuteBatch(UpdateVersionQuery + _applicationVersion.Major);
#endif
        }

        private void UpgradeData(int dbVersion, IRepository repo)
        {
            if (dbVersion <= 7)
            {
                repo.ExecuteBatch(UpdateQueries.UpdateCastingCostForUltimateNightmare);
            }
        }
    }
}
