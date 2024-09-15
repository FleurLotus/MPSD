namespace MagicPictureSetDownloader.ScryFall
{
    using System.Collections.Generic;

    using Common.Web;

    public interface IPriceImporter
    {
        IEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param);
        IReadOnlyList<KeyValuePair<string, object>> GetDefaultCardUrls(WebAccess webAccess);
    }
}
