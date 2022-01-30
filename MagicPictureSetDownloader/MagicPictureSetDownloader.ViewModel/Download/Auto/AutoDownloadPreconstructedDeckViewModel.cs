namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Core.Deck;

    public class AutoDownloadPreconstructedDeckViewModel : AutoDownloadViewModelBase
    {
        private readonly PreconstructedDeckImporter _preconstructedDeckImporter;

        public AutoDownloadPreconstructedDeckViewModel()
            : base("Download new preconstructed decks")
        {
            _preconstructedDeckImporter = new PreconstructedDeckImporter(GetExtraInfo);
        }
        protected override IReadOnlyList<KeyValuePair<string, object>> GetUrls()
        {
            return DownloadManager.GetPreconstructedDecksUrls(_preconstructedDeckImporter);
        }
        protected override string Download(string url, object param)
        {
            return DownloadManager.InsertPreconstructedDeckCardsInDb(url, _preconstructedDeckImporter);
        }
        private string GetExtraInfo(string url)
        {
            return DownloadManager.GetExtraInfo(url);
        }
    }
}