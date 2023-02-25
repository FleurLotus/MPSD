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
#if !DEBUG
        private const string UpdateVersionQuery = "UPDATE Version Set Major = ";
#endif
        private readonly string _connectionString;
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

                foreach (string code in new[] { "'CNS", "EXO" })
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
                    repo.ExecuteParametrizeCommand(UpdateQueries.
                        AddNoneGathererSets,
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

                //12.4
                foreach (string code in new[] { "C19" })
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.CorrectHasFoilFalse,
                       new KeyValuePair<string, object>("@code", code));
                }

                //12.5
                Dictionary<string, Tuple<string, string>> vehicleCorrections = new Dictionary<string, Tuple<string, string>>
                {
                    {"Aethersphere Harvester", new Tuple<string, string>("3", "5")},
                    {"Aradara Express", new Tuple<string, string>("8", "6")},
                    {"Ballista Charger", new Tuple<string, string>("6", "6")},
                    {"Bomat Bazaar Barge", new Tuple<string, string>("5", "5")},
                    {"Conqueror's Galleon", new Tuple<string, string>("2", "10")},
                    {"Consulate Dreadnought", new Tuple<string, string>("7", "11")},
                    {"Cultivator's Caravan", new Tuple<string, string>("5", "5")},
                    {"Daredevil Dragster", new Tuple<string, string>("4", "4")},
                    {"Demolition Stomper", new Tuple<string, string>("10", "7")},
                    {"Dusk Legion Dreadnought", new Tuple<string, string>("4", "6")},
                    {"Enchanted Carriage", new Tuple<string, string>("4", "4")},
                    {"Fell Flagship", new Tuple<string, string>("3", "3")},
                    {"Fleetwheel Cruiser", new Tuple<string, string>("5", "3")},
                    {"Heart of Kiran", new Tuple<string, string>("4", "4")},
                    {"Irontread Crusher", new Tuple<string, string>("6", "6")},
                    {"Mizzium Tank", new Tuple<string, string>("3", "2")},
                    {"Mobile Garrison", new Tuple<string, string>("3", "4")},
                    {"Ovalchase Dragster", new Tuple<string, string>("6", "1")},
                    {"Parhelion II", new Tuple<string, string>("5", "5")},
                    {"Peacewalker Colossus", new Tuple<string, string>("6", "6")},
                    {"Renegade Freighter", new Tuple<string, string>("4", "3")},
                    {"Shadowed Caravel", new Tuple<string, string>("2", "2")},
                    {"Silent Submersible", new Tuple<string, string>("2", "3")},
                    {"Sky Skiff", new Tuple<string, string>("2", "3")},
                    {"Skysovereign, Consul Flagship", new Tuple<string, string>("6", "5")},
                    {"Sleek Schooner", new Tuple<string, string>("4", "3")},
                    {"Smuggler's Copter", new Tuple<string, string>("3", "3")},
                    {"Untethered Express", new Tuple<string, string>("4", "4")},
                    {"Weatherlight", new Tuple<string, string>("4", "5")}
                };

                foreach (var kv in vehicleCorrections)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.CorrectVehicle,
                       new KeyValuePair<string, object>("@name", kv.Key),
                       new KeyValuePair<string, object>("@power", kv.Value.Item1),
                       new KeyValuePair<string, object>("@toughness", kv.Value.Item2));
                }
                //12.6
                repo.ExecuteBatch(UpdateQueries.CorrectMystericBoosterText);
            }

            if (dbVersion <= 13)
            {
                //13.0
                if (!repo.ColumnExists("CardEditionsInCollection", "AltArtNumber"))
                {
                    repo.ExecuteBatch(UpdateQueries.AddAltArtColumnToCollection);
                }
                if (!repo.ColumnExists("CardEditionsInCollection", "FoilAltArtNumber"))
                {
                    repo.ExecuteBatch(UpdateQueries.AddFoilAltArtColumnToCollection);
                }
                if (!repo.ColumnExists("Audit", "IsAltArt"))
                {
                    repo.ExecuteBatch(UpdateQueries.AddIsAltArtColumnToAudit);
                    repo.ExecuteBatch(UpdateQueries.UpdateAltArtColumnOfAudit);
                }

                if (!repo.TableExists("CardEditionVariation"))
                {
                    repo.ExecuteBatch(UpdateQueries.CreateCardEditionVariationTable);
                }

                //13.2
                repo.ExecuteParametrizeCommand(UpdateQueries.UpdateSecretLairDropMissingCard,
                    new KeyValuePair<string, object>("@value", 99));


                //13.4
                if (!repo.TableExists("BackSideModalDoubleFacedCard"))
                {
                    repo.ExecuteBatch(UpdateQueries.CreateBackSideModalDoubleFacedCardTable);
                }

                repo.ExecuteParametrizeCommand(UpdateQueries.UpdateSecretLairDropMissingCard,
                    new KeyValuePair<string, object>("@value", 122));

                HashSet<string> reverseSideOfFlipLand = new HashSet<string>(new[]
                { 
                    //Zendikar Rising
                    "Agadeem, the Undercrypt", "Akoum Teeth", "Bala Ged Sanctuary", "Beyeen Coast", 
                    "Blackbloom Bog", "Boulderloft Pathway", "Emeria, Shattered Skyclave", "Glasspool Shore", 
                    "Grimclimb Pathway", "Hagra Broodpit", "Jwari Ruins", "Kabira Plateau", 
                    "Kazandu Valley", "Kazuul's Cliffs", "Khalni Territory","Lavaglide Pathway", 
                    "Makindi Mesas", "Malakir Mire", "Murkwater Pathway", "Ondu Skyruins", 
                    "Pelakka Caverns", "Pillarverge Pathway", "Sea Gate, Reborn", "Sejiri Glacier", 
                    "Shatterskull, the Hammer Pass", "Silundi Isle", "Skyclave Basilica", "Song-Mad Ruins", 
                    "Spikefield Cave", "Tangled Vale", "Timbercrown Pathway", "Turntimber, Serpentine Wood", 
                    "Umara Skyfalls", "Vastwood Thicket", "Valakut Stoneforge", "Zof Bloodbog",
                    //Kaldheim
                    "Hakka, Whispering Raven", "Harnfel, Horn of Bounty", "Kaldring, the Rimestaff", "Mistgate Pathway", 
                    "Searstep Pathway", "Slitherbore Pathway", "Sword of the Realms", "Tergrid's Lantern", 
                    "The Omenkeel", "The Prismatic Bridge", "The Ringhart Crest", "Throne of Death", 
                    "Tibalt, Cosmic Impostor", "Tidechannel Pathway", "Toralf's Hammer", "Valkmira, Protector's Shield",
                    //StixHaven: School of Mages
                    "Augusta, Dean of Order", "Awaken the Blood Avatar", "Deadly Vanity", "Echoing Equation", 
                    "Embrose, Dean of Shadow", "Explore the Vastlands", "Flamethrower Sonata", "Imbraham, Dean of Theory", 
                    "Journey to the Oracle", "Lisette, Dean of the Root", "Lukka, Wayward Bonder", "Nassari, Dean of Expression", 
                    "Restorative Burst", "Revel in Silence", "Search for Blex", "Will, Scholar of Frost",
                });
                foreach (string s in reverseSideOfFlipLand)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.InsertBackSideModalDoubleFacedCard,
                       new KeyValuePair<string, object>("@name", s));
                }
            }

            if (dbVersion <= 14)
            {
                //14.1
                repo.ExecuteBatch(UpdateQueries.CorrectAECardName);
                repo.ExecuteBatch(UpdateQueries.CorrectAECardPartName);
                repo.ExecuteBatch(UpdateQueries.CorrectKillDestroyCard);

                //14.2
                repo.ExecuteBatch(UpdateQueries.InsertMissingCardIndulgeExcess1);
                repo.ExecuteBatch(UpdateQueries.InsertMissingCardIndulgeExcess2);
                repo.ExecuteBatch(UpdateQueries.InsertMissingCardIndulgeExcess3);

                Dictionary<string, Tuple<string, string>> specialSets = new Dictionary<string, Tuple<string, string>> {
                    { "Q06" , Tuple.Create("Pioneer Challenger Decks 2021", "2022-01-01 00:00:00") }
                };

                foreach (var kv in specialSets)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.AddNoneGathererSetsWithDate,
                       new KeyValuePair<string, object>("@code", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value.Item1),
                       new KeyValuePair<string, object>("@date", kv.Value.Item2));
                }

                //14.3
                Dictionary<string, Tuple<string, string>> specialSets2 = new Dictionary<string, Tuple<string, string>> {
                    { "SCD" , Tuple.Create("Starter Commander Decks", "2022-12-02 00:00:00") },
                    { "PHED" , Tuple.Create("Heads I Win, Tails You Lose", "2022-04-22 00:00:00") }
                };

                foreach (var kv in specialSets2)
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.AddNoneGathererSetsWithDate,
                       new KeyValuePair<string, object>("@code", kv.Key),
                       new KeyValuePair<string, object>("@name", kv.Value.Item1),
                       new KeyValuePair<string, object>("@date", kv.Value.Item2));
                }

            }

            using (var temporaryDabase = new TemporaryDatabase())
            {
                AddPreconstructedDeckFromReference(repo, temporaryDabase.ConnectionString);
                AddCardEditionVariationFromReference(repo, temporaryDabase.ConnectionString);
            }
        }
        private void AddCardEditionVariationFromReference(IRepository repo, string connectionString)
        {
            Dictionary<int, List<Tuple<int, string>>> referenceCardEditionVariations = GetCardEditionVariations(connectionString);
            Dictionary<int, List<Tuple<int, string>>> currentCardEditionVariations = GetCardEditionVariations(_connectionString);

            var parameters = new List<KeyValuePair<string, object>[]>();

            foreach (var kv in referenceCardEditionVariations)
            {
                if (!currentCardEditionVariations.ContainsKey(kv.Key))
                {
                    foreach (var tuple in kv.Value)
                    {
                        parameters.Add(new KeyValuePair<string, object>[] {
                            new KeyValuePair<string, object>("@idGatherer", kv.Key),
                            new KeyValuePair<string, object>("@otherIdGatherer", tuple.Item1),
                            new KeyValuePair<string, object>("@url", tuple.Item2)});
                    }
                }
            }
            if (parameters.Count > 0)
            {
                repo.ExecuteParametrizeCommandMulti(UpdateQueries.InsertNewCardEditionVariation, parameters);
                parameters.Clear();
            }
        }
        private Dictionary<int, List<Tuple<int, string>>> GetCardEditionVariations(string connectionString)
        {
            Dictionary<int, List<Tuple<int, string>>> ret = new Dictionary<int, List<Tuple<int, string>>>();

            using (SQLiteConnection cnx = new SQLiteConnection(connectionString))
            {
                cnx.Open();
                using (DbCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = UpdateQueries.SelectCardEditionVariation;

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idGatherer = reader.GetInt32(0);
                            if (!ret.TryGetValue(idGatherer, out List<Tuple<int, string>> list))
                            {
                                list = new List<Tuple<int, string>>();
                                ret.Add(idGatherer, list);
                            }

                            list.Add(new Tuple<int, string>(reader.GetInt32(1), reader.GetString(2)));
                        }
                    }
                }
            }
            return ret;
        }

        private void AddPreconstructedDeckFromReference(IRepository repo, string connectionString)
        {
            Dictionary<string, Tuple<string, string>> referencePreconstructedDecks = GetPreconstructedDecks(connectionString);
            Dictionary<int, string[]> referenceFakeIdGathererCardEdition = GetFakeIdGathererCardEdition(connectionString);
            Dictionary<Tuple<int, string, string>, int> referencePreconstructedDeckCards = GetPreconstructedDeckCards(connectionString);

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
    }
}
