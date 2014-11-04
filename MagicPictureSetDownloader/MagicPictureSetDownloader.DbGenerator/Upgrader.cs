namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Linq;
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
            int version = -1;
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
                if (!repo.TableExists("Language"))
                {
                    repo.ExecuteBatch(@"
CREATE TABLE [Language] (
  [Id] int NOT NULL
, [Name] nvarchar(50) NOT NULL
);
GO
ALTER TABLE [Language] ADD CONSTRAINT [PK_Language] PRIMARY KEY ([Id]);
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (1,N'English');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (2,N'Italian');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (4,N'French');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (8,N'German');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (16,N'Portuguese');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (32,N'Spanish');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (64,N'Japanese');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (128,N'Korean');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (256,N'Traditional Chinese');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (512,N'Simplified Chinese');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (1024,N'Russian');
GO"
                        );
                }
                //Update queries

            }
        }
        private void ExecuteUpgradeCommandsForPicture(Repository repo, int version)
        {
            if (version <= 1)
            {
                //Update queries
            }
        }
    }
}
