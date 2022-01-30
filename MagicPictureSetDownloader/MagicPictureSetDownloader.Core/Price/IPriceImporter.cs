namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;

    using Common.Web;

    public interface IPriceImporter
    {
        IReadOnlyList<KeyValuePair<string, object>> GetUrls(WebAccess webAccess);
        IEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param, out string errorMessage);
    }
}
