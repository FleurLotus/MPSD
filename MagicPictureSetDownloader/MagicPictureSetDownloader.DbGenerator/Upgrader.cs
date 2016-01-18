namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Collections.Generic;
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
            if (dbVersion <= 7)
            {
                //7.1
                repo.ExecuteBatch(UpdateQueries.UpdateCastingCostForUltimateNightmare);

                //7.4
                repo.ExecuteBatch(UpdateQueries.UpdateCommander2015Code);

                repo.ExecuteBatch(UpdateQueries.UpdateEditionWithNoCard);

                repo.ExecuteBatch(UpdateQueries.UpdateEditionWithSpecialCard);
                
                foreach (Tuple<string, string> t in GetAlternativeCodeToUpdate())
                {
                    repo.ExecuteBatch(UpdateQueries.UpdateEditionAlternativeCode, t.Item1, t.Item2);
                }

                foreach (Tuple<string, string> t in GetForceAlternativeCodeToUpdate())
                {
                    repo.ExecuteBatch(UpdateQueries.UpdateForceEditionAlternativeCode, t.Item1, t.Item2);
                }

                //7.7
                repo.ExecuteBatch(UpdateQueries.UpdateZendikarExpeditionCount);
            }
        }
        private void UpgradePicture(int dbVersion, IRepository repo)
        {
            if (dbVersion <= 7)
            {
                //7.4
                foreach (Tuple<string, string> t in GetImageToCopy())
                {
                    repo.ExecuteBatch(UpdateQueries.CopyImage, t.Item1, t.Item2);
                }
                //7.5
                if (!repo.RowExists(null, "TreePicture", new[] { "Name" }, new object[] { "@C" }))
                {
                    repo.ExecuteParametrizeCommand(UpdateQueries.InsertNewTreePicture,
                        new KeyValuePair<string, object>("@name", "@C"),
                        new KeyValuePair<string, object>("@value",
                            new byte[]
                                {
                                    0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x0f, 0x08, 0x06, 0x00,
                                    0x00, 0x00, 0x3b, 0xd6, 0x95, 0x4a, 0x00, 0x00, 0x00, 0x01, 0x73, 0x52, 0x47, 0x42, 0x00, 0xae, 0xce, 0x1c, 0xe9, 0x00, 0x00, 0x00, 0x04, 0x67, 0x41, 0x4d, 0x41,
                                    0x00, 0x00, 0xb1, 0x8f, 0x0b, 0xfc, 0x61, 0x05, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc4, 0x00, 0x00, 0x0e, 0xc4, 0x01, 0x95, 0x2b,
                                    0x0e, 0x1b, 0x00, 0x00, 0x01, 0x04, 0x49, 0x44, 0x41, 0x54, 0x38, 0x4f, 0x75, 0x92, 0x31, 0x0a, 0xc2, 0x40, 0x10, 0x45, 0x73, 0x17, 0x2d, 0xbc, 0x4b, 0xc0, 0x0b,
                                    0xa4, 0xb2, 0x4c, 0x63, 0x2b, 0x16, 0x36, 0xda, 0x49, 0xbc, 0x80, 0x9a, 0xc6, 0x2e, 0x60, 0xb0, 0x50, 0x41, 0x85, 0x9c, 0x6d, 0xe4, 0x0d, 0xfc, 0x65, 0x12, 0xe3,
                                    0xc0, 0xcf, 0x66, 0x77, 0xf6, 0xef, 0xff, 0x33, 0xbb, 0x99, 0x8d, 0xc4, 0xa1, 0xaa, 0xec, 0x79, 0x7f, 0x58, 0xd7, 0x75, 0x76, 0x6b, 0x5b, 0x9f, 0x8f, 0x45, 0x8f,
                                    0x7c, 0x3e, 0x9e, 0x9c, 0x00, 0x3e, 0xef, 0x97, 0x93, 0x34, 0x07, 0xe4, 0x63, 0x24, 0x32, 0x0a, 0x71, 0xe3, 0x66, 0xbd, 0xb2, 0xd9, 0x64, 0x6a, 0xd7, 0xa6, 0xe9,
                                    0xad, 0xe3, 0x48, 0xe1, 0xe4, 0xa8, 0x08, 0x2e, 0x75, 0x6d, 0xf3, 0x3c, 0x77, 0x32, 0x23, 0x2e, 0x62, 0x5e, 0x0e, 0x9c, 0x1c, 0x13, 0x28, 0x41, 0x58, 0x14, 0x85,
                                    0xbb, 0xe1, 0x00, 0xfe, 0xe3, 0x1e, 0xe0, 0xe4, 0x58, 0x17, 0x27, 0x42, 0x04, 0x5a, 0xe3, 0x30, 0x0e, 0x58, 0x96, 0x65, 0x5a, 0x03, 0xf0, 0x32, 0xd5, 0x4a, 0x52,
                                    0x36, 0xd5, 0x69, 0x41, 0x39, 0x1c, 0x28, 0xc7, 0x98, 0xf1, 0x03, 0xe1, 0x5f, 0x7d, 0x02, 0x4a, 0xec, 0x01, 0xea, 0x51, 0x52, 0x16, 0x99, 0x66, 0x45, 0x92, 0x80,
                                    0xaa, 0xd4, 0x99, 0xbb, 0x72, 0xac, 0x59, 0x1b, 0x62, 0xf7, 0x71, 0x22, 0x67, 0xfb, 0xdd, 0x36, 0xad, 0x7b, 0xcd, 0xc3, 0x6e, 0xeb, 0x7e, 0xe5, 0x40, 0xf5, 0x0e,
                                    0xef, 0xdb, 0xbb, 0xcd, 0x67, 0x78, 0xcf, 0x10, 0x50, 0xd3, 0xc8, 0x41, 0xb1, 0x17, 0xbd, 0x7b, 0x26, 0xe2, 0x0b, 0x63, 0x23, 0x6a, 0x00, 0x27, 0x91, 0xf8, 0xf3,
                                    0xc2, 0x14, 0xd1, 0x81, 0x5e, 0x59, 0xbc, 0x36, 0x29, 0x2a, 0x7a, 0x64, 0x05, 0xcd, 0x80, 0x44, 0x83, 0x18, 0x99, 0xff, 0x86, 0xd9, 0x17, 0x49, 0xdb, 0xa8, 0xc7,
                                    0x1a, 0x88, 0xc7, 0xdc, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60, 0x82
                                }));
                }
            }
        }

        private IEnumerable<Tuple<string, string>> GetImageToCopy()
        {
            yield return new Tuple<string, string>("Time Spiral", "Time Spiral \"Timeshifted\"");
            yield return new Tuple<string, string>("Archenemy", "Scheme");
            yield return new Tuple<string, string>("Planechase", "Phenomenon");
            yield return new Tuple<string, string>("Planechase", "Plane");
        }
        private IEnumerable<Tuple<string, string>> GetAlternativeCodeToUpdate()
        {
            yield return new Tuple<string, string>("3B", "Revised Edition");
            yield return new Tuple<string, string>("3W", "Revised Edition");
            yield return new Tuple<string, string>("AH", "Archenemy");
            yield return new Tuple<string, string>("BT", "Beatdown Box Set");
            yield return new Tuple<string, string>("BZ", "Battle for Zendikar");
            yield return new Tuple<string, string>("C5", "Commander 2015");
            yield return new Tuple<string, string>("DC", "Duel Decks: Zendikar vs. Eldrazi");
            yield return new Tuple<string, string>("DH", "Speed vs. Cunning");
            yield return new Tuple<string, string>("DJ", "Elspeth vs. Kiora");
            yield return new Tuple<string, string>("HP", "Promo set for Gatherer");
            yield return new Tuple<string, string>("ME", "Masters Edition");
            yield return new Tuple<string, string>("MU", "Modern Masters 2015 Edition");
            yield return new Tuple<string, string>("OR", "Magic Origins");
            yield return new Tuple<string, string>("RE", "Fourth Edition");
        }
        private IEnumerable<Tuple<string, string>> GetForceAlternativeCodeToUpdate()
        {
            yield return new Tuple<string, string>("V1", "From the Vault: Dragons");
            yield return new Tuple<string, string>("V2", "From the Vault: Exiled");
            yield return new Tuple<string, string>("V3", "From the Vault: Relics");
            yield return new Tuple<string, string>("V4", "From the Vault: Legends");
            yield return new Tuple<string, string>("V5", "From the Vault: Realms");
            yield return new Tuple<string, string>("V6", "From the Vault: Twenty");
            yield return new Tuple<string, string>("V7", "From the Vault: Annihilation");
            yield return new Tuple<string, string>("V8", "From the Vault: Angels");
            yield return new Tuple<string, string>("", "Masters Edition II");
            yield return new Tuple<string, string>("R3", "Graveborn");
            yield return new Tuple<string, string>("R2", "Slivers");
            yield return new Tuple<string, string>("TD", "Time Spiral \"Timeshifted\"");
            yield return new Tuple<string, string>("TS", "Time Spiral");
        }
    }
}
