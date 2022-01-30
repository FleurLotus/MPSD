namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System.Collections.Generic;
    public class AutoDownloadRuleViewModel : AutoDownloadViewModelBase
    {
        public AutoDownloadRuleViewModel()
            : base("Download new rules")
        {
        }
        protected override IReadOnlyList<KeyValuePair<string, object>> GetUrls()
        {
            return DownloadManager.GetRulesUrls();
        }
        protected override string Download(string url, object param)
        {
            return DownloadManager.InsertRuleInDb(url, param);
        }
    }
}