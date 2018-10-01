namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Reflection;
    using System.Collections.Generic;
    using System.IO;

    using Common.SQL;
    using Common.SQLite;

    public class Upgrader
    {
        private const string VersionQuery = "SELECT Major FROM Version";
        private const string UpdateVersionQuery = "UPDATE Version Set Major = ";
        private readonly string _connectionString;
        private readonly DatabaseType _data;
        private static readonly Version _applicationVersion;

        static Upgrader()
        {
            Assembly entryAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = entryAssembly.GetName();
            _applicationVersion = assemblyName.Version;
        }
        internal Upgrader(string connectionString, DatabaseType data)
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
            {
                throw new Exception("Db is newer that application");
            }

            //We redo the change of current version because of minor version upgrade
            /*
                        if (_applicationVersion.Major == dbVersion)
                            return;
            */

            if (dbVersion < 6)
            {
                throw new Exception("You have udpated the version!!! There is no released version with this db version");
            }

            Repository repo = new Repository(_connectionString);
            switch (_data)
            {
                case DatabaseType.Data:
                    UpgradeData(dbVersion, repo);
                    break;
                case DatabaseType.Picture:
                    UpgradePicture(dbVersion, repo);
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
            if (dbVersion <= 8)
            {
                //8.0
                if (!repo.TableExists("Ruling"))
                {
                    repo.ExecuteBatch(UpdateQueries.CreateRulingTable);
                }

                repo.ExecuteBatch(UpdateQueries.RemoveDuelDeckFromName);
                repo.ExecuteBatch(UpdateQueries.UpdateCodeHeroesMonsterDeck);

                //8.14
                repo.ExecuteBatch(UpdateQueries.UpdateKaladeshInventionGathererName);

                //8.15
                repo.ExecuteBatch(UpdateQueries.DeleteKaladeshInventionGathererIdChange);

            }
            if (dbVersion <= 9)
            {
                repo.ExecuteBatch(UpdateQueries.UpdateKaladeshInventionMissingCard);

                // 9.1
                repo.ExecuteBatch(UpdateQueries.UpdateKaladeshInventionBlock);

                Dictionary<string, string> missing = new Dictionary<string, string> {
                    { "EXP" , "Zendikar Expeditions"},
                    { "W16" , "Welcome Deck 2016"},
                    { "MPS" , "Masterpiece Series: Kaladesh Inventions"},
                };

                foreach (var kv in missing)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.UpdateEditionMissingCode,
                       new KeyValuePair<string, object>("@code", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value));
                }

                if (repo.ColumnExists("Card", "Loyalty") && repo.GetTable("Card").GetColumn("Loyalty").DataType == "INTEGER")
                {
                    foreach (string query in UpdateQueries.ChangeCardLoyaltyColumnType)
                    {
                        repo.ExecuteBatch(query);
                    }
                }

                // 9.2
                repo.ExecuteBatch(UpdateQueries.UpdateAmonkhetInvocationsMissingCard);

                // 9.3
                repo.ExecuteBatch(UpdateQueries.UpdateHourofDevastationReleaseDate);
            }
        }
        private void UpgradePicture(int dbVersion, IRepository repo)
        {
            AddMissingPictureFromReference(repo);
        }
        private void AddMissingPictureFromReference(IRepository repo)
        {
            string temporaryDatabasePath = new Generator(DatabaseType.Picture).Generate(true);
            temporaryDatabasePath = Path.Combine(temporaryDatabasePath, DatabaseGenerator.GetResourceName(DatabaseType.Picture));
            string connectionString = (new SQLiteConnectionStringBuilder { DataSource = temporaryDatabasePath }).ToString();

            Dictionary<string, byte[]> reference = GetTreePictures(connectionString);
            File.Delete(temporaryDatabasePath);

            Dictionary<string, byte[]> current = GetTreePictures(_connectionString);
            
            foreach (var kv in reference) 
            {
                if (!current.ContainsKey(kv.Key))
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.InsertNewTreePicture,
                       new KeyValuePair<string, object>("@name", kv.Key),
                       new KeyValuePair<string, object>("@value", kv.Value));
                }
            }
        }
        private Dictionary<string, byte[]> GetTreePictures(string connectionString)
        {
            Dictionary<string, byte[]> ret = new Dictionary<string, byte[]>(StringComparer.InvariantCultureIgnoreCase);
           
            using (SQLiteConnection cnx = new SQLiteConnection(connectionString))
            {
                cnx.Open();
                using (DbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = UpdateQueries.SelectTreePicture;

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(reader.GetString(0), (byte[])reader.GetValue(1));
                        }
                    }
                }
            }
            return ret;
        }
    }
}
