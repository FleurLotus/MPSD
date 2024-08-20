namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    using Common.Web;

    using MagicPictureSetDownloader.ScryFall.JsonData;
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public static class ScryFallDataRetriever 
    {
        private const string ScryfallBulk = @"https://api.scryfall.com/bulk-data";
        private const string ScryfallSets = @"https://api.scryfall.com/sets";
        private const string DefaultCard = "default_cards";
        private const string AllCard = "all_cards";

        private static BulkDataList GetBulkData(WebAccess webAccess)
        {
            string json = webAccess.GetHtml(ScryfallBulk);
            return JsonSerializer.Deserialize<BulkDataList>(json);
        }
        public static Set[] GetBulkSets(WebAccess webAccess)
        {
            string json = webAccess.GetHtml(ScryfallSets);
            return JsonSerializer.Deserialize<FullSet[]>(json).Select(fs => fs.ToSet()).ToArray();
        }

        private static IReadOnlyList<KeyValuePair<string, BulkData>> GetUrls(WebAccess webAccess, string type)
        {
            IList<KeyValuePair<string, BulkData>> urls = new List<KeyValuePair<string, BulkData>>();

            BulkDataList bulkDataList = GetBulkData(webAccess);

            BulkData bulkData = bulkDataList.Data.FirstOrDefault(d => d.Type == type);
            if (bulkData != null)
            {
                urls.Add(new KeyValuePair<string, BulkData>(bulkData.DownloadUri, bulkData));
            }
            return urls.ToArray();
        }
        public static IReadOnlyList<KeyValuePair<string, BulkData>> GetDefaultCardUrls(WebAccess webAccess)
        {
            return GetUrls(webAccess, DefaultCard);
        }
        public static IReadOnlyList<KeyValuePair<string, BulkData>> GetAllCardUrls(WebAccess webAccess)
        {
            return GetUrls(webAccess, AllCard);
        }
        internal static FullCard[] GetCardsInfoFromBulk(WebAccess webAccess, BulkData bulkData)
        {
            string filePath = Path.Combine(Path.GetTempPath(), $"{DefaultCard}.json");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                webAccess.DownloadFile(bulkData.DownloadUri, filePath);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    return JsonSerializer.Deserialize<FullCard[]>(fileStream);
                }
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
        public static Card[] GetCardsInfo(WebAccess webAccess, out BulkData bulkData)
        {
            bulkData = null;
            IReadOnlyList<KeyValuePair<string, BulkData>> info = GetDefaultCardUrls(webAccess);
            if (info == null || info.Count < 1)
            {
                return Array.Empty<Card>();
            }

            bulkData = info[0].Value;
            return GetCardsInfoFromBulk(webAccess, bulkData).Select(fc => fc.ToCard()).ToArray();
        }
    }
}
