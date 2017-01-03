namespace MagicPictureSetDownloader.DbGenerator
{
    using System;

    public static class DatabaseGenerator
    {
        public static void Generate(DatabaseType databaseType)
        {
            new Generator(databaseType).Generate();
        }
        public static void VersionVerify(string connectionString, DatabaseType databaseType)
        {
            new Upgrader(connectionString, databaseType).Upgrade();
        }
        public static string GetResourceName(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.Data:
                    return "MagicData.sqlite";
                case DatabaseType.Picture:
                    return "MagicPicture.sqlite";
                default:
                    throw new ArgumentException("Unknown DatabaseType type");
            }
        }

    }
}
