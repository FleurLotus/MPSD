namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Reflection;

    using Common.SQLCE;

    public class Upgrader
    {
        private const string VersionQuery = "SELECT Major FROM Version";
        private const string UpdateVersionQuery = "UPDATE Version Set Major = ";
        private readonly string _connectionString;
        private readonly DbType _data;
        private static readonly Version _applicationVersion;

        static Upgrader()
        {
            Assembly entryAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = entryAssembly.GetName();
            _applicationVersion = assemblyName.Version;
        }
        internal Upgrader(string connectionString, DbType data)
        {
            _connectionString = connectionString;
            _data = data;
        }

        internal void Upgrade()
        {
            int version;
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                using (SqlCeCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = VersionQuery;

                    version = (int)(cmd.ExecuteScalar());
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

            if (dbVersion < 4)
                throw new Exception("You have udpated the version!!! There is no released version with this db version");


            Repository repo = new Repository(_connectionString);
            switch (_data)
            {
                case DbType.Data:
                    ExecuteUpgradeCommandsForData(repo, dbVersion);
                    break;
                case DbType.Picture:
                    ExecuteUpgradeCommandsForPicture(repo, dbVersion);
                    break;
            }
        }
        private void ExecuteUpgradeCommandsForData(Repository repo, int dbVersion)
        {
            if (dbVersion <= 4)
            {
                repo.ExecuteBatch(UpdateVersionQuery + "4");
            }
        }
        private void ExecuteUpgradeCommandsForPicture(Repository repo, int dbVersion)
        {
            if (dbVersion <= 4)
            {
                repo.ExecuteBatch(UpdateVersionQuery + "4");
            }
        }
    }
}
