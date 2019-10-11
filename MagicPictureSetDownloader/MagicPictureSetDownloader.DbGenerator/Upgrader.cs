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
            {
                repo.ExecuteBatch(UpdateVersionQuery + _applicationVersion.Major);
            }
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

            if (dbVersion <= 10)
            {
                //10.7
                Dictionary<string, string> updatedcode = new Dictionary<string, string> {
                    { "UGL" , "Unglued"},
                    { "CM2" , "Commander Anthology 2018"},
                    { "V10" , "From the Vault: Relics"},
                    { "V11" , "From the Vault: Legends"},
                    { "V12" , "From the Vault: Realms"},
                    { "H09" , "Premium Deck Series: Sliverss"},
                    { "PD3" , "Premium Deck Series: Graveborn"},
                    { "TSP" , "Time Spiral"},
                    { "TSB" , @"Time Spiral ""Timeshifted"""},
                };

                foreach (var kv in updatedcode)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.UpdateEditionMissingCode,
                       new KeyValuePair<string, object>("@code", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value));
                }

                foreach (string query in UpdateQueries.RemoveWrongCardFromGuildOfRavnica)
                {
                    repo.ExecuteBatch(query);
                }
            }

            if (dbVersion <= 11)
            {
                //11.0
                if (!repo.TableExists("Price"))
                {
                    repo.ExecuteBatch(UpdateQueries.CreatePriceTable);
                }

                foreach (string code in new[] { "'CNS","EXO" })
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.CorrectHasFoilTrue,
                       new KeyValuePair<string, object>("@code", code));
                }

                foreach (string code in new[] { "V15", "V16", "V17", "C15", "C16", "CMA", "C17", "CM2", "C18", "DDO", "DDP", "DDQ", "DDR", "DDS", "DDT", "DDU", "GS1", "E01", "PCA" })
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.CorrectHasFoilFalse,
                       new KeyValuePair<string, object>("@code", code));
                }

                //11.1
                repo.ExecuteBatch(UpdateQueries.CorrectBattleBondPartnerNotFlipCard);
            }
            if (dbVersion <= 12)
            {
                //12.0
                if (!repo.TableExists("PreconstructedDeck"))
                {
                    repo.ExecuteBatch(UpdateQueries.CreatePreconstructedDeckTable);
                }
                if (!repo.TableExists("PreconstructedDeckCardEdition"))
                {
                    repo.ExecuteBatch(UpdateQueries.CreatePreconstructedDeckCardEditionTable);
                }

                Dictionary<string, string> updatedcode = new Dictionary<string, string> {
                    { "MD1" , "Modern Event Deck 2014"},
                    { "DD1" , "Duel Decks: Elves vs. Goblins"},
                    { "DVD" , "Duel Decks Anthology, Divine vs. Demonic"},
                    { "GVL" , "Duel Decks Anthology, Garruk vs. Liliana"},
                    { "JVC" , "Duel Decks Anthology, Jace vs. Chandra"},
                    { "EVG" , "Duel Decks Anthology, Elves vs. Goblins"},
                };

                foreach (var kv in updatedcode)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.UpdateEditionMissingCode,
                       new KeyValuePair<string, object>("@code", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value));
                }

                Dictionary<string, string> specialSets = new Dictionary<string, string> {
                    { "CP1" , "Magic 2015 Clash Pack"},
                    { "CP2" , "Fate Reforged Clash Pack"},
                    { "CP3" , "Magic Origins Clash Packs"},
                    { "TD2" , "Mirrodin Pure vs. New Phyrexia"},
                    { "DPA" , "Duels of the Planeswalkers"},
                    { "DKM" , "Deckmasters"},
                    { "ATH" , "Anthologies"},
                    { "ITP" , "Introductory Two-Player Set"},
                };

                foreach (var kv in specialSets)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.AddNoneGathererSets,
                       new KeyValuePair<string, object>("@code", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value));
                }

                Dictionary<string, string> alternativeCodes = new Dictionary<string, string> {
                    { "OE01" , "Archenemy: Nicol Bolas"},
                    { "OPC2" , "Planechase 2012 Edition"},
                    { "OARC" , "Archenemy"},
                    { "H09" , "Slivers"},
                    { "OHOP" , "Planechase"},
                    { "P02" , "Portal Second Age"},
                };

                foreach (var kv in alternativeCodes)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.AddAlternativeCode,
                       new KeyValuePair<string, object>("@code", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value));
                }

                foreach (string code in new[] { "C19" })
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.CorrectHasFoilFalse,
                       new KeyValuePair<string, object>("@code", code));
                }
            }

            AddPreconstructedDeckFromReference(repo);
        }
        private void AddPreconstructedDeckFromReference(IRepository repo)
        {
            string temporaryDatabasePath = new Generator(DatabaseType.Data).Generate(true);
            temporaryDatabasePath = Path.Combine(temporaryDatabasePath, DatabaseGenerator.GetResourceName(DatabaseType.Data));
            string connectionString = (new SQLiteConnectionStringBuilder { DataSource = temporaryDatabasePath }).ToString();

            Dictionary<string, Tuple<string, string>> referencePreconstructedDecks = GetPreconstructedDecks(connectionString);
            Dictionary<int, string[]> referenceFakeIdGathererCardEdition = GetFakeIdGathererCardEdition(connectionString);
            Dictionary<Tuple<int, string, string>, int> referencePreconstructedDeckCards = GetPreconstructedDeckCards(connectionString);

            File.Delete(temporaryDatabasePath);

            Dictionary<string, Tuple<string, string>> currentPreconstructedDecks = GetPreconstructedDecks(_connectionString);
            Dictionary<int, string[]> currentFakeIdGathererCardEdition = GetFakeIdGathererCardEdition(_connectionString);
            Dictionary<Tuple<int, string, string>, int> currentPreconstructedDeckCards = GetPreconstructedDeckCards(_connectionString);

            var parameters = new List<KeyValuePair<string, object>[]>();

            foreach (var kv in referencePreconstructedDecks)
            {
                if (!currentPreconstructedDecks.ContainsKey(kv.Key))
                {
                    parameters.Add(new KeyValuePair<string, object>[] {
                        new KeyValuePair<string, object>("@url", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value.Item1),
                       new KeyValuePair<string, object>("@gatherername", kv.Value.Item2)});
                }
            }
            if (parameters.Count > 0)
            {
                repo.ExecuteParametrizeCommandMulti(UpdateQueries.InsertNewPreconstuctedDecks, parameters);
                parameters.Clear();
            }

            foreach (var kv in referenceFakeIdGathererCardEdition)
            {
                if (!currentFakeIdGathererCardEdition.ContainsKey(kv.Key))
                {
                    parameters.Add(new KeyValuePair<string, object>[] {
                       new KeyValuePair<string, object>("@idgatherer", kv.Key),
                       new KeyValuePair<string, object>("@gatherername", kv.Value[0]),
                       new KeyValuePair<string, object>("@raritycode", kv.Value[1]),
                       new KeyValuePair<string, object>("@cardname", kv.Value[2]),
                       new KeyValuePair<string, object>("@cardpartname", kv.Value[3]),
                       new KeyValuePair<string, object>("@url", kv.Value[4]) });
                }
            }
            if (parameters.Count > 0)
            {
                repo.ExecuteParametrizeCommandMulti(UpdateQueries.InsertFakeIdGathererCardEdition, parameters);
                parameters.Clear();
            }
            foreach (var kv in referencePreconstructedDeckCards)
            {
                if (!currentPreconstructedDeckCards.ContainsKey(kv.Key))
                {
                    parameters.Add(new KeyValuePair<string, object>[] {
                       new KeyValuePair<string, object>("@idgatherer", kv.Key.Item1),
                       new KeyValuePair<string, object>("@name", kv.Key.Item2),
                       new KeyValuePair<string, object>("@gatherername", kv.Key.Item3),
                       new KeyValuePair<string, object>("@number", kv.Value) });
                }
            }
            if (parameters.Count > 0)
            {
                repo.ExecuteParametrizeCommandMulti(UpdateQueries.InsertPreconstructedDeckCards, parameters);
                parameters.Clear();
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
        private Dictionary<Tuple<int, string, string>, int> GetPreconstructedDeckCards(string connectionString)
        {
            Dictionary<Tuple<int, string, string>, int> ret = new Dictionary<Tuple<int, string, string>, int>();

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
                            ret.Add(new Tuple<int, string, string>(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)), reader.GetInt32(3));
                        }
                    }
                }
            }
            return ret;
        }
        private Dictionary<int, string[]> GetFakeIdGathererCardEdition(string connectionString)
        {
            Dictionary<int, string[]> ret = new Dictionary<int, string[]>();

            using (SQLiteConnection cnx = new SQLiteConnection(connectionString))
            {
                cnx.Open();
                using (DbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = UpdateQueries.SelectFakeIdGathererCardEdition;

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(reader.GetInt32(0), new string[] { reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5) });
                        }
                    }
                }
            }
            return ret;
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
