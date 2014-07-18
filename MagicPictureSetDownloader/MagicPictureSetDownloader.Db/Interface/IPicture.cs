namespace MagicPictureSetDownloader.Db
{
    public interface IPicture
    {
        int IdGatherer { get; }
        byte[] Image { get; }
        byte[] FoilImage { get; }
    }
}