namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class AutoDownloadImageViewModel : AutoDownloadViewModelBase
    {
        public AutoDownloadImageViewModel()
            : base("Download new images")
        {
        }
        protected override IAsyncEnumerable<(string url, object param)> GetUrls(CancellationToken ct)
        {
            return DownloadManager.GetMissingPictureUrls(ct);
        }
        protected override async Task<string> Download(string url, object param, CancellationToken ct)
        {
            return await DownloadManager.InsertPictureInDb(url, param, ct).ConfigureAwait(false);
        }
    }
}