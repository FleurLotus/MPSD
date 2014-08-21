namespace MagicPictureSetDownloader.DbGenerator
{
    public static class DatabaseGenerator
    {
        public static void GenerateMagicData(string connectionString)
        {
            new GeneratorBase(connectionString).Generate(Properties.Resource.MagicData);
        }
        public static void GenerateMagicPicture(string connectionString)
        {
            new GeneratorBase(connectionString).Generate(Properties.Resource.MagicPicture);
        }

    }
}
