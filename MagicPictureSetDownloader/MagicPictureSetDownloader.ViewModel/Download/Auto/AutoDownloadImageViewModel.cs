namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    public class AutoDownloadImageViewModel : AutoDownloadViewModelBase
    {
        public AutoDownloadImageViewModel()
            : base("Download new images")
        {
        }
        protected override string[] GetUrls()
        {
            return DownloadManager.GetMissingPictureUrls();
        }
        protected override void Download(string url)
        {
            DownloadManager.InsertPictureInDb(url);
        }
    }
}