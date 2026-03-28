namespace MagicPictureSetDownloader.ScryFall
{
    using System.Collections.Generic;
    using System.Threading;

    using Common.Web;

    public interface IPriceImporter
    {
        IAsyncEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param, CancellationToken ct);
        IAsyncEnumerable<(string url, object param)> GetDefaultCardUrls(WebAccess webAccess, CancellationToken ct);
    }
}