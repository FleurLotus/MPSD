namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;
    using System.Collections.Generic;

    public interface IPriceImporter
    {
        PriceSource PriceSource { get; }
        string[] GetUrls();
        IEnumerable<PriceInfo> Parse(string text, out string errorMessage);
    }
}
