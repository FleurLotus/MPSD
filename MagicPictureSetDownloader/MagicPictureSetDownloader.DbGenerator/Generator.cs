namespace MagicPictureSetDownloader.DbGenerator
{
    using System.Data.SqlServerCe;
    using System.IO;

    using Common.SQLCE;
    using Common.Zip;

    internal class Generator
    {
        private readonly string _connectionString;
        private readonly DatabasebType _data;

        internal Generator(string connectionString, DatabasebType data)
        {
            _connectionString = connectionString;
            _data = data;
            SqlCeEngine engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();
        }
        internal void Generate()
        {
            StreamReader sr = new StreamReader(Zipper.UnZipOneFile(GetStream()));
            new Repository(_connectionString).ExecuteBatch(sr.ReadToEnd());
        }
        private byte[] GetStream()
        {
            switch (_data)
            {
                case DatabasebType.Data:
                    return Properties.Resource.MagicData;
                case DatabasebType.Picture:
                    return Properties.Resource.MagicPicture;
            }

            return null;
        }
    }
}
