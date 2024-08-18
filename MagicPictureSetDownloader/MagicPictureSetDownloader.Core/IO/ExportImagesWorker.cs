namespace MagicPictureSetDownloader.Core.IO
{
    using System;

    using System.IO;
    using System.Text.RegularExpressions;
    using Common.Drawing;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class ExportImagesWorker
    {
        private readonly IMagicDatabaseReadAndWriteReference MagicDatabase = MagicDatabaseManager.ReadAndWriteReference;
        private readonly IPictureDatabaseMigration PictureDatabase = MagicDatabaseManager.ReadOnly.PictureDatabaseMigration;

        public Tuple<bool, object>[] GetAllPicture()
        {
            return PictureDatabase.GetMigrationPictures();
        }
        public void Export(bool forTree, object id)
        {
            if (forTree)
            {
                ITreePicture treePicture = PictureDatabase.LoadTreePicture(id.ToString());
                if (treePicture != null)
                {
                    MagicDatabase.InsertNewTreePicture(treePicture.Name, treePicture.Image);
                }
            }
            else if (id is string i)
            {
                IPicture picture = PictureDatabase.LoadPicture(i);
                if (picture != null)
                {
                    MagicDatabase.InsertNewPicture(picture.IdScryFall, picture.Image);
                }
            }
        }
    }
}
