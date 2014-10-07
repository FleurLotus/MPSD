namespace MagicPictureSetDownloader.DbGenerator
{
    public static class DatabaseGenerator
    {
        public static void GenerateMagicData(string connectionString)
        {
            new Generator(connectionString, DbType.Data).Generate();
        }
        public static void GenerateMagicPicture(string connectionString)
        {
            new Generator(connectionString, DbType.Picture).Generate();
        }

        public static void VersionVerifyMagicData(string connectionString)
        {
            new Upgrader(connectionString, DbType.Data).Upgrade();
        }
        public static void VersionVerifyMagicPicture(string connectionString)
        {
            new Upgrader(connectionString, DbType.Picture).Upgrade();
        }

    }
}
