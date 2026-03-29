namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;

    using Common.SQL;
    using Common.SQLite;

    using MagicPictureSetDownloader.Interface;

    using Microsoft.Extensions.Logging;

    public class Upgrader
    {
        private const string VersionQuery = "SELECT Major FROM Version";
#if !DEBUG
        private const string UpdateVersionQuery = "UPDATE Version Set Major = ";
#endif
        private readonly string _connectionString;
        private readonly ILogger Logger = LogManager.Factory.CreateLogger<Upgrader>();
        private static readonly Version _applicationVersion;

        private class TemporaryDatabase : IDisposable
        {
            private readonly string _temporaryDatabasePath;

            public TemporaryDatabase()
            {
                _temporaryDatabasePath = new Generator().Generate(true);
                _temporaryDatabasePath = Path.Combine(_temporaryDatabasePath, DatabaseGenerator.GetResourceName());
                ConnectionString = (new SQLiteConnectionStringBuilder { DataSource = _temporaryDatabasePath }).ToString();
            }
            public string ConnectionString { get; }

            public void Dispose()
            {
                File.Delete(_temporaryDatabasePath);
            }
        }

        static Upgrader()
        {
            Assembly entryAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = entryAssembly.GetName();
            _applicationVersion = assemblyName.Version;
        }
        internal Upgrader(string connectionString)
        {
            _connectionString = connectionString;
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

                    version = (int) (long) (cmd.ExecuteScalar());
                }
            }
            try
            {
                ExecuteUpgradeCommands(version);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while upgrading database");
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

            if (dbVersion < 21)
            {
                throw new Exception("Your current database version is incompatible with the new release, you will lose data.");
            }

            Repository repo = new Repository(_connectionString);

            UpgradeData(dbVersion, repo);

#if !DEBUG
            //No update of Version in debug
            if (_applicationVersion.Major != dbVersion)
            {
                repo.ExecuteBatch(UpdateVersionQuery + _applicationVersion.Major);
            }
#endif
        }

        private void UpgradeData(int dbVersion, IRepository repo)
        {
            using (TemporaryDatabase temporaryDabase = new TemporaryDatabase())
            {
                AddPreconstructedDeckFromReference(repo, temporaryDabase.ConnectionString);
            }

            if (dbVersion <= 21)
            {
            }
        }
        private void AddPreconstructedDeckFromReference(IRepository repo, string connectionString)
        {
            Dictionary<string, Tuple<string, string>> referencePreconstructedDecks = GetPreconstructedDecks(connectionString);
            Dictionary<Tuple<string, string, string>, int> referencePreconstructedDeckCards = GetPreconstructedDeckCards(connectionString);

            Dictionary<string, Tuple<string, string>> currentPreconstructedDecks = GetPreconstructedDecks(_connectionString);
            Dictionary<Tuple<string, string, string>, int> currentPreconstructedDeckCards = GetPreconstructedDeckCards(_connectionString);

            List<KeyValuePair<string, object>[]> parameters = new List<KeyValuePair<string, object>[]>();

            foreach (KeyValuePair<string, Tuple<string, string>> kv in referencePreconstructedDecks)
            {
                if (!currentPreconstructedDecks.ContainsKey(kv.Key))
                {
                    parameters.Add(new KeyValuePair<string, object>[] {
                       new KeyValuePair<string, object>("@url", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value.Item1),
                       new KeyValuePair<string, object>("@editionName", kv.Value.Item2)});
                }
            }
            if (parameters.Count > 0)
            {
                repo.ExecuteParametrizeCommandMulti(UpdateQueries.InsertNewPreconstuctedDecks, parameters);
                parameters.Clear();
            }

            foreach (KeyValuePair<Tuple<string, string, string>, int> kv in referencePreconstructedDeckCards)
            {
                if (!currentPreconstructedDeckCards.ContainsKey(kv.Key))
                {
                    parameters.Add(new KeyValuePair<string, object>[] {
                       new KeyValuePair<string, object>("@idScryFall", kv.Key.Item1),
                       new KeyValuePair<string, object>("@name", kv.Key.Item2),
                       new KeyValuePair<string, object>("@editionName", kv.Key.Item3),
                       new KeyValuePair<string, object>("@number", kv.Value) });
                }
            }
            if (parameters.Count > 0)
            {
                repo.ExecuteParametrizeCommandMulti(UpdateQueries.InsertPreconstructedDeckCards, parameters);
                parameters.Clear();
            }
        }
        private Dictionary<string, Tuple<string, string>> GetPreconstructedDecks(string connectionString)
        {
            Dictionary<string, Tuple<string, string>> ret = new Dictionary<string, Tuple<string, string>>(StringComparer.InvariantCultureIgnoreCase);

            using (SQLiteConnection cnx = new SQLiteConnection(connectionString))
            {
                cnx.Open();
                using (DbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = UpdateQueries.SelectPreconstuctedDecks;

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(reader.GetString(0), new Tuple<string, string>(reader.GetString(1), reader.GetString(2)));
                        }
                    }
                }
            }
            return ret;
        }
        private Dictionary<Tuple<string, string, string>, int> GetPreconstructedDeckCards(string connectionString)
        {
            Dictionary<Tuple<string, string, string>, int> ret = new Dictionary<Tuple<string, string, string>, int>();

            using (SQLiteConnection cnx = new SQLiteConnection(connectionString))
            {
                cnx.Open();
                using (DbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = UpdateQueries.SelectPreconstuctedDeckCards;

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(new Tuple<string, string, string>(reader.GetString(0), reader.GetString(1), reader.GetString(2)), reader.GetInt32(3));
                        }
                    }
                }
            }
            return ret;
        }
    }
}