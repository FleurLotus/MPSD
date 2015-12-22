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
                repo.ExecuteBatch(UpdateQueries.UpdateCastingCostForUltimateNightmare);

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

            }
        }
        private void UpgradePicture(int dbVersion, IRepository repo)
        {
            if (dbVersion <= 7)
            {
                foreach (Tuple<string, string> t in GetImageToCopy())
                {
                    repo.ExecuteBatch(UpdateQueries.CopyImage, t.Item1, t.Item2);
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
