namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using MagicPictureSetDownloader.Core.Deck;

    public class AutoDownloadPreconstructedDeckViewModel : AutoDownloadViewModelBase
    {
        private readonly PreconstructedDeckImporter _preconstructedDeckImporter;


        public AutoDownloadPreconstructedDeckViewModel()
            : base("Download new preconstructed decks")
        {
            _preconstructedDeckImporter = new PreconstructedDeckImporter();
        }
        protected override string[] GetUrls()
        {
            return DownloadManager.GetPreconstructedDecksUrls(_preconstructedDeckImporter);
        }
        protected override void Download(string url)
        {
            DownloadManager.InsertPreconstructedDeckCardsInDb(url, _preconstructedDeckImporter);
        }
    }
}