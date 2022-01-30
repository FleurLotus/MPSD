namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System.Collections.Generic;

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
        protected override IReadOnlyList<KeyValuePair<string, object>> GetUrls()
        {
            return DownloadManager.GetPricesUrls(_priceImporter);
        }
        protected override string Download(string url, object param)
        {
            return DownloadManager.InsertPriceInDb(_priceImporter, url, param);
        }
    }
}