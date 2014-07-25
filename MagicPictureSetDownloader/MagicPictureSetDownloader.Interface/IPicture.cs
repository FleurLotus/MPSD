namespace MagicPictureSetDownloader.Interface
{
    public interface IPicture
    {
        int IdGatherer { get; }
        byte[] Image { get; }
    }
}