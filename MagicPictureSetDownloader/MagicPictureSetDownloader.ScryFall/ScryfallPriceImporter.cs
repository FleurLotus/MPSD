namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Web;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall.JsonData;

    internal class ScryFallPriceImporter : IPriceImporter
    {
        public async IAsyncEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param, [EnumeratorCancellation] CancellationToken ct)
        {
            BulkData bulkData = (BulkData) param;

            await foreach (FullCard fullcard in ScryFallDataRetriever.GetCardsInfoFromBulk(webAccess, bulkData, ct).ConfigureAwait(false))
            {
                foreach (PriceInfo price in ExtractCardPrice(fullcard, bulkData.UpdatedAt))
                {
                    yield return price;
                }
            }
        }
        public async IAsyncEnumerable<(string url, object param)> GetDefaultCardUrls(WebAccess webAccess, [EnumeratorCancellation] CancellationToken ct)
        {
            BulkData bulkData = await ScryFallDataRetriever.GetCardUrls(webAccess, false, ct).ConfigureAwait(false);

            ct.ThrowIfCancellationRequested();
            yield return (bulkData.DownloadUri, bulkData);
        }
        private IEnumerable<PriceInfo> ExtractCardPrice(FullCard scryfallCard, DateTime updatedAt)
        {
            if (scryfallCard.Prices == null)
            {
                yield break;
            }
            int p;
            if (double.TryParse(scryfallCard.Prices.Usd, out double price))
            {
                p = (int) (price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.TCGplayer, Foil = false, Value = p };
            }
            if (double.TryParse(scryfallCard.Prices.UsdFoil, out price))
            {
                p = (int) (price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.TCGplayer, Foil = true, Value = p };
            }
            if (double.TryParse(scryfallCard.Prices.Eur, out price))
            {
                p = (int) (price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.Cardmarket, Foil = false, Value = p };
            }
            if (double.TryParse(scryfallCard.Prices.EurFoil, out price))
            {
                p = (int) (price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.Cardmarket, Foil = true, Value = p };
            }
        }
    }
}