namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Web;

    using MagicPictureSetDownloader.ScryFall.JsonData;
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public static class ScryFallDataRetriever
    {
        private const string ScryfallBulk = @"https://api.scryfall.com/bulk-data";
        private const string ScryfallSets = @"https://api.scryfall.com/sets";
        private const string DefaultCard = "default_cards";
        private const string AllCard = "all_cards";

        private static async Task<BulkDataList> GetBulkData(WebAccess webAccess, CancellationToken ct)
        {
            string json = await webAccess.GetHtmlAsync(ScryfallBulk, false, ct).ConfigureAwait(false);
            return JsonSerializer.Deserialize<BulkDataList>(json);
        }
        public static async Task<Set[]> GetBulkSets(WebAccess webAccess, CancellationToken ct)
        {
            string json = await webAccess.GetHtmlAsync(ScryfallSets, false, ct).ConfigureAwait(false);
            return JsonSerializer.Deserialize<AllSet>(json).Data.Select(fs => fs.ToSet()).ToArray();
        }

        private static async Task<BulkData> GetUrls(WebAccess webAccess, string type, CancellationToken ct)
        {
            BulkDataList bulkDataList = await GetBulkData(webAccess, ct).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            return bulkDataList.Data.FirstOrDefault(d => d.Type == type);
        }
        public static async Task<BulkData> GetCardUrls(WebAccess webAccess, bool allCards, CancellationToken ct)
        {
            return await GetUrls(webAccess, allCards ? AllCard : DefaultCard, ct).ConfigureAwait(false);
        }
        internal static async IAsyncEnumerable<FullCard> GetCardsInfoFromBulk(WebAccess webAccess, BulkData bulkData, [EnumeratorCancellation] CancellationToken ct)
        {
            string origfileName = Path.GetFileName(bulkData.DownloadUri);
            string fileName = Path.GetFileNameWithoutExtension(origfileName) + "_" + bulkData.Id + Path.GetExtension(origfileName);
            string fileNametemplate = Path.GetFileNameWithoutExtension(origfileName) + "_*" + Path.GetExtension(origfileName);
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            foreach (string file in Directory.GetFiles(Path.GetTempPath(), fileNametemplate).Where(f => string.Compare(f, filePath, true) != 0))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // Ignore any errors when trying to delete old files.
                }
            }

            if (!File.Exists(filePath))
            {
                try
                {
                    await webAccess.DownloadFileAsync(bulkData.DownloadUri, filePath, ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    //The file is likely incomplete, so delete it if it exists.
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch
                        {
                        }
                    }
                    throw;
                }
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
            {
                await foreach (FullCard card in JsonSerializer.DeserializeAsyncEnumerable<FullCard>(fileStream, cancellationToken: ct).ConfigureAwait(false))
                {

#if DEBUG
                    IList<string> errors = JsonMissingMapping.Check(card);
                    if (errors.Count > 0)
                    {
                        Debugger.Break();
                    }
#endif
                    yield return card;
                }
            }
        }
        public static async IAsyncEnumerable<Card> GetCardsInfo(WebAccess webAccess, bool allCards, [EnumeratorCancellation] CancellationToken ct)
        {
            BulkData bukData = await GetCardUrls(webAccess, allCards, ct).ConfigureAwait(false);
            if (bukData == null)
            {
                yield break;
            }

            await foreach (FullCard card in GetCardsInfoFromBulk(webAccess, bukData, ct).ConfigureAwait(false))
            {

                yield return card.ToCard();
            }
        }
    }
}