 namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;

    public static class PriceImporterFactory
    {
        public static IPriceImporter Create(PriceSource pricesource)
        {
            return pricesource switch
            {
                PriceSource.Scryfall => new ScryfallPriceImporter(),
                _ => throw new PriceImporterException("Unknown PriceSource type:" + pricesource),
            };
        }
    }
}
