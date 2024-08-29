namespace MagicPictureSetDownloader.Interface
{
    public interface ITreePicture
    {
        string Name { get; }
        string FilePath { get; }
        byte[] Image { get; }
    }
}