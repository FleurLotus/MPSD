namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    public class AutoDownloadRuleViewModel : AutoDownloadViewModelBase
    {
        public AutoDownloadRuleViewModel()
            : base("Download new rules")
        {
        }
        protected override string[] GetUrls()
        {
            return DownloadManager.GetRulesUrls();
        }
        protected override string Download(string url)
        {
            return DownloadManager.InsertRuleInDb(url);
        }
    }
}