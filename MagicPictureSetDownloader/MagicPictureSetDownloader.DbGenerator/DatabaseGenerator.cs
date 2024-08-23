namespace MagicPictureSetDownloader.DbGenerator
{
    public static class DatabaseGenerator
    {
        public static void Generate()
        {
            new Generator().Generate();
        }
        public static void GeneratePictures()
        {
            new Generator().GeneratePictures();
        }
        public static void VersionVerify(string connectionString)
        {
            new Upgrader(connectionString).Upgrade();
        }
        public static string GetResourceName()
        {
            return "MagicDataScryFall.sqlite";
        }
    }
}
