namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using MagicPictureSetDownloader.Core.Deck;

    public class AutoDownloadPreconstructedDeckViewModel : AutoDownloadViewModelBase
    {
        private readonly PreconstructedDeckImporter _preconstructedDeckImporter;

        public AutoDownloadPreconstructedDeckViewModel()
            : base("Download new preconstructed decks")
        {
            _preconstructedDeckImporter = new PreconstructedDeckImporter(GetExtraInfo);
        }
        protected override string[] GetUrls()
        {
            return DownloadManager.GetPreconstructedDecksUrls(_preconstructedDeckImporter);
        }
        protected override string Download(string url)
        {
            return DownloadManager.InsertPreconstructedDeckCardsInDb(url, _preconstructedDeckImporter);
        }
        private string GetExtraInfo(string url)
        {
            return DownloadManager.GetExtraInfo(url);
        }
    }
}