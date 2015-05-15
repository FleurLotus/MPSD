namespace MagicPictureSetDownloader.DbGenerator
{
    public static class DatabaseGenerator
    {
        public static void Generate(string connectionString, DatabasebType databaseType)
        {
            new Generator(connectionString, databaseType).Generate();
        }
        public static void VersionVerify(string connectionString, DatabasebType databaseType)
        {
            new Upgrader(connectionString, databaseType).Upgrade();
        }
    }
}
