namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;

    public interface IPriceImporter: IParser<PriceInfo>
    {
        PriceSource PriceSource { get; }
        string[] GetUrls();
    }
}
