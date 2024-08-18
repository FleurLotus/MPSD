namespace MagicPictureSetDownloader.ScryFall
{
    using System.Collections.Generic;

    using Common.Web;

    using MagicPictureSetDownloader.ScryFall.JsonData;

    public interface IPriceImporter
    {
        IReadOnlyList<KeyValuePair<string, object>> GetDefaultCardUrls(WebAccess webAccess);
        IReadOnlyList<KeyValuePair<string, object>> GetAllCardUrls(WebAccess webAccess);
        FullSet[] GetBulkSets(WebAccess webAccess);
        IEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param, out string errorMessage);
    }
}
