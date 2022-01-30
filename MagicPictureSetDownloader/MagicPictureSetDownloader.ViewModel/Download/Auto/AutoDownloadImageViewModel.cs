namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System.Collections.Generic;
    public class AutoDownloadImageViewModel : AutoDownloadViewModelBase
    {
        public AutoDownloadImageViewModel()
            : base("Download new images")
        {
        }
        protected override IReadOnlyList<KeyValuePair<string, object>> GetUrls()
        {
            return DownloadManager.GetMissingPictureUrls();
        }
        protected override string Download(string url, object param)
        {
            return DownloadManager.InsertPictureInDb(url, param);
        }
    }
}