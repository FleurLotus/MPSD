namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Common.Database;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal class PictureDatabaseMigration : IPictureDatabaseMigration
    {
        private readonly string _connectionString;
        internal PictureDatabaseMigration()
        {
            string fileName = "MagicPicture.sqlite";
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);

            CouldMigrate = File.Exists(filePath);

            _connectionString = (new SQLiteConnectionStringBuilder { DataSource = filePath }).ToString();
        }
        public bool CouldMigrate { get; }

        public Tuple<bool, object>[] GetMigrationPictures()
        {
            if (!CouldMigrate)
            {
                return Array.Empty<Tuple<bool, object>>();
            }

            using (IDbConnection cnx = GetPictureConnectionInternal())
            {
                return Mapper<TreePicture>.LoadAll(cnx).Select(tp => new Tuple<bool, object>(true, tp.Name)).Union(
                       Mapper<PictureKey>.LoadAll(cnx).Select(pk => new Tuple<bool, object>(false, pk.IdScryFall))).ToArray();
            }
        }
        private IDbConnection GetPictureConnectionInternal()
        {
            SQLiteConnection cnx = new SQLiteConnection(_connectionString);
            cnx.Open();
            return cnx;
        }
        public IPicture LoadPicture(string idScryFall)
        {
            if (!CouldMigrate)
            {
                return null;
            }

            Picture picture = new Picture { IdScryFall = idScryFall };

            using (IDbConnection cnx = GetPictureConnectionInternal())
            {
                return Mapper<Picture>.Load(cnx, picture);
            }
        }
        public ITreePicture LoadTreePicture(string name)
        {
            if (!CouldMigrate)
            {
                return null;
            }

            TreePicture treePicture = new TreePicture { Name = name };

            using (IDbConnection cnx = GetPictureConnectionInternal())
            {
                return Mapper<TreePicture>.Load(cnx, treePicture);
            }
        }
    }
}
