 namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;

    public static class PriceImporterFactory
    {
        public static IPriceImporter Create(PriceSource pricesource)
        {
            switch (pricesource)
            {
                case PriceSource.MTGGoldfish:
                    return new MTGGoldfishPriceImporter();

                default:
                    throw new PriceImporterException("Unknown PriceSource type:" + pricesource);
            }
        }
    }
}
