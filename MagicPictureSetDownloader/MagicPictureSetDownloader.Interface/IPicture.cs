namespace MagicPictureSetDownloader.Interface
{
    public interface IPicture : IPictureKey
    {
        byte[] Image { get; }
    }
}