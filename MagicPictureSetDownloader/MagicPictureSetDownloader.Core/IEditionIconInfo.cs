namespace MagicPictureSetDownloader.Core
{
    public interface IEditionIconInfo
    {
        string Name { get; }
        string Url { get; }
        byte[] Icon { get; }
        string Code { get; }
    }
}