namespace MagicPictureSetDownloader.Interface
{
    using System;
    public interface IPictureDatabaseMigration
    {
        bool CouldMigrate { get; }
        Tuple<bool, object>[] GetMigrationPictures();
        IPicture LoadPicture(string idScryFall);
        ITreePicture LoadTreePicture(string name);
    }
}
