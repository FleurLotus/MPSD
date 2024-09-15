namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Web;
   
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall.JsonData;

    internal class ScryFallPriceImporter : IPriceImporter
    {
        public IEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param)
        {
            BulkData bulkData = (BulkData)param;

            FullCard[] cards = ScryFallDataRetriever.GetCardsInfoFromBulk(webAccess, bulkData);

            List<PriceInfo> ret = cards.SelectMany(c => ExtractCardPrice(c, bulkData.UpdatedAt)).ToList();
            return ret;
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetDefaultCardUrls(WebAccess webAccess)
        {
            return new List<KeyValuePair<string, object>>(ScryFallDataRetriever.GetDefaultCardUrls(webAccess).Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)));
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
                p = (int)(price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.TCGplayer, Foil = false, Value = p };
            }
            if (double.TryParse(scryfallCard.Prices.UsdFoil, out price))
            {
                p = (int)(price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.TCGplayer, Foil = true, Value = p };
            }
            if (double.TryParse(scryfallCard.Prices.Eur, out price))
            {
                p = (int)(price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.Cardmarket, Foil = false, Value = p };
            }
            if (double.TryParse(scryfallCard.Prices.EurFoil, out price))
            {
                p = (int)(price * 100);
                yield return new PriceInfo { UpdateDate = updatedAt, IdScryFall = scryfallCard.Id.ToString(), PriceSource = PriceValueSource.Cardmarket, Foil = true, Value = p };
            }
        }
    }
}
