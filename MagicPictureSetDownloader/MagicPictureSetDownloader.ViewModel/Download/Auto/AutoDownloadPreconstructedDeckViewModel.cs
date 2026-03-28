namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using MagicPictureSetDownloader.Core.Deck;

    public class AutoDownloadPreconstructedDeckViewModel : AutoDownloadViewModelBase
    {
        private readonly PreconstructedDeckImporter _preconstructedDeckImporter;

        public AutoDownloadPreconstructedDeckViewModel()
            : base("Download new preconstructed decks")
        {
            _preconstructedDeckImporter = new PreconstructedDeckImporter();
        }
        protected override IAsyncEnumerable<(string url, object param)> GetUrls(CancellationToken ct)
        {
            return DownloadManager.GetPreconstructedDecksUrls(_preconstructedDeckImporter, ct);
        }
        protected override async Task<string> Download(string url, object param, CancellationToken ct)
        {
            return await DownloadManager.InsertPreconstructedDeckCardsInDb(url, _preconstructedDeckImporter, ct).ConfigureAwait(false);
        }
    }
}