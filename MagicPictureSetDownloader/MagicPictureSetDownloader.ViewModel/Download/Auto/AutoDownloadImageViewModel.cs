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
        protected override string Download(string url)
        {
            return DownloadManager.InsertPictureInDb(url);
        }
    }
}