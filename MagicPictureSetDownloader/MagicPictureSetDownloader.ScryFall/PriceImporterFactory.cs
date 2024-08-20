namespace MagicPictureSetDownloader.ScryFall
{
    using MagicPictureSetDownloader.Interface;

    public static class PriceImporterFactory
    {
        public static IPriceImporter Create(PriceSource pricesource)
        {
            return pricesource switch
            {
                PriceSource.Scryfall => new ScryFallPriceImporter(),
                _ => throw new PriceImporterException("Unknown PriceSource type:" + pricesource),
            };
        }
    }
}
