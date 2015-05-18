namespace MagicPictureSetDownloader.DbGenerator
{
    using System;

    public static class DatabaseGenerator
    {
        public static void Generate(DatabasebType databaseType)
        {
            new Generator(databaseType).Generate();
        }
        public static void VersionVerify(string connectionString, DatabasebType databaseType)
        {
            new Upgrader(connectionString, databaseType).Upgrade();
        }
        public static string GetResourceName(DatabasebType databaseType)
        {
            switch (databaseType)
            {
                case DatabasebType.Data:
                    return "MagicData.sqlite";
                case DatabasebType.Picture:
                    return "MagicPicture.sqlite";
                default:
                    throw new ArgumentException("Unknown DatabasebType type");
            }
        }

    }
}
