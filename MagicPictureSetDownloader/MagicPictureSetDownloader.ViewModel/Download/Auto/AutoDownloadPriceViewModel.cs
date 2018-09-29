namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Core;

    public class AutoDownloadPriceViewModel : AutoDownloadViewModelBase
    {
        private readonly IPriceImporter _priceImporter;

        public AutoDownloadPriceViewModel(PriceSource priceSource)
            : base("Download new price")
        {
            _priceImporter = PriceImporterFactory.Create(priceSource);
        }
        protected override string[] GetUrls()
        {
            return DownloadManager.GetPricesUrls(_priceImporter);
        }
        protected override void Download(string url)
        {
            DownloadManager.InsertPriceInDb(_priceImporter, url);
        }
    }
}