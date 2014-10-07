namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Linq;
    using System.Reflection;

    public class Upgrader
    {
        private const string VersionQuery = "SELECT Major FROM Version";
        private const string UpdateVersionQuery = "UPDATE Version Set Major = ";
        private readonly string _connectionString;
        private readonly DbType _data;
        private static readonly int _expectedVersion;

        static Upgrader()
        {
            Assembly entryAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = entryAssembly.GetName();
            _expectedVersion = assemblyName.Version.Major;
        }
        internal Upgrader(string connectionString, DbType data)
        {
            _connectionString = connectionString;
            _data = data;
        }

        internal void Upgrade()
        {
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                using (SqlCeCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = VersionQuery;

                    int version = (int)(cmd.ExecuteScalar());
                    foreach (string command in GetUpgradeCommands(version))
                    {
                        string trimcommand = command.TrimEnd(new[] { '\r', '\n' });
                        if (!string.IsNullOrWhiteSpace(trimcommand))
                        {
                            cmd.CommandText = trimcommand;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private IEnumerable<string> GetUpgradeCommands(int version)
        {
            if (_expectedVersion < version)
                throw new Exception("Db is newer that application");

            if (_expectedVersion == version)
                return Enumerable.Empty<string>();

            switch (_data)
            {
                case DbType.Data:
                    return GetUpgradeCommandsForData(version);
                case DbType.Picture:
                    return GetUpgradeCommandsForPicture(version);
            }

            return Enumerable.Empty<string>();
        }
        private IEnumerable<string> GetUpgradeCommandsForData(int version)
        {
            if (version < 1)
            {
                //Update queries
            }

            yield return UpdateVersionQuery + _expectedVersion;
        }
        private IEnumerable<string> GetUpgradeCommandsForPicture(int version)
        {
            if (version < 1)
            {
                //Update queries
            }

            yield return UpdateVersionQuery + _expectedVersion;
        }
    }
}
