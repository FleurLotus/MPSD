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

        private void ExecuteUpgradeCommands(int version)
        {
            if (_expectedVersion < version)
                throw new Exception("Db is newer that application");

            if (_expectedVersion == version)
                return;

            Repository repo = new Repository(_connectionString);
            switch (_data)
            {
                case DbType.Data:
                    ExecuteUpgradeCommandsForData(repo, version);
                    break;
                case DbType.Picture:
                    ExecuteUpgradeCommandsForPicture(repo, version);
                    break;
            }
            repo.ExecuteBatch(UpdateVersionQuery + _expectedVersion);
        }
        private void ExecuteUpgradeCommandsForData(Repository repo, int version)
        {
            if (version <= 2)
            {
                //Update queries
                if (!repo.TableExists("Language"))
                    repo.ExecuteBatch(UpdateQueries.CreateLanguageTable);

                if (!repo.TableExists("Translate"))
                    repo.ExecuteBatch(UpdateQueries.CreateTranslateTable);

                if (!repo.ColumnExists("CardEditionsInCollection", "IdLanguage"))
                    repo.ExecuteBatch(UpdateQueries.AddColumnLanguageToCardEditionsInCollectionQuery);

                repo.ExecuteBatch(UpdateQueries.UpdateAlaraBlockReleaseDateQuery);
                repo.ExecuteBatch(UpdateQueries.RemoveCompleteSetQuery);
            }
            if (version <= 3)
            {
                if (!repo.RowExists(null, "Rarity", new[] { "Name" }, new object[] { "Promo" }))
                    repo.ExecuteBatch(UpdateQueries.InsertPromoRarity);

                if (repo.ColumnExists("Card", "CastingCost") && repo.GetTable("Card").GetColumn("CastingCost").CharacterMaxLength < 100)
                    repo.ExecuteBatch(UpdateQueries.ExtendCardCastingCostLength);

                if (repo.ColumnExists("Card", "Name") && repo.GetTable("Card").GetColumn("Name").CharacterMaxLength < 150)
                    repo.ExecuteBatch(UpdateQueries.ExtendCardNameLength);

                if (repo.ColumnExists("Translate", "Name") && repo.GetTable("Translate").GetColumn("Name").CharacterMaxLength < 150)
                    repo.ExecuteBatch(UpdateQueries.ExtendTranslateNameLength);

                if (repo.RowExists(null, "Block", new[] { "Name", "Id" }, new object[] { "Others", -6 }))
                    repo.ExecuteBatch(UpdateQueries.NewBlock);

                if (repo.RowExists(null, "Card", new[] { "Name", "CastingCost" }, new object[] { "Little Girl", "@500" }))
                    repo.ExecuteBatch(UpdateQueries.UpdateLittleGirl);

            }
        }
        private void ExecuteUpgradeCommandsForPicture(Repository repo, int version)
        {
            /*
            if (version <= 2)
            {
                //Update queries
            }
            */
        }
    }
}
