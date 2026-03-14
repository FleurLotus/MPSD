namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall;

    public class AutoDownloadPriceViewModel : AutoDownloadViewModelBase
    {
        private readonly IPriceImporter _priceImporter;

        public AutoDownloadPriceViewModel(PriceSource priceSource)
            : base("Download new price")
        {
            _priceImporter = PriceImporterFactory.Create(priceSource);
        }
        protected override IAsyncEnumerable<(string url, object param)> GetUrls(CancellationToken ct)
        {
            return DownloadManager.GetPricesUrls(_priceImporter, ct);
        }
        protected override async Task<string> Download(string url, object param, CancellationToken ct)
        {
            return await DownloadManager.InsertPriceInDb(_priceImporter, url, param, ct).ConfigureAwait(false);
        }
    }
}