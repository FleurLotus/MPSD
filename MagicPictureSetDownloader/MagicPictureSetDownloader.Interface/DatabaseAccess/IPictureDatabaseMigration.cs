namespace MagicPictureSetDownloader.Interface
{
    using System;
    public interface IPictureDatabaseMigration
    {
        bool CouldMigrate { get; }
        Tuple<bool, object>[] GetMigrationPictures();
        IPicture LoadPicture(int idGatherer);
        ITreePicture LoadTreePicture(string name);
    }
}
