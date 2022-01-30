 namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;

    public static class PriceImporterFactory
    {
        public static IPriceImporter Create(PriceSource pricesource)
        {
            switch (pricesource)
            {
                case PriceSource.Scryfall:
                    return new ScryfallPriceImporter();
                default:
                    throw new PriceImporterException("Unknown PriceSource type:" + pricesource);
            }
        }
    }
}
