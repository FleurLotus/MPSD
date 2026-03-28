namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Notify;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public class DownloadLanguageViewModel : DownloadViewModelBase
    {
        private bool _disposed;
        private ScryFallCardTransformer _scryFallCardTransformer;

        public DownloadLanguageViewModel()
            : base("Download Language")
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {

            }
            _disposed = true;

            base.Dispose(disposing);
        }

        protected override Task<bool> StartImpl(CancellationToken ct)
        {
            _ = Task.Run(() => GetJsonData(ct), ct);
            return Task.FromResult(true);
        }

        private async Task GetJsonData(CancellationToken ct)
        {
            try
            {
                Card[] cards = await DownloadManager.GetCards(true, ct).ToArrayAsync(ct).ConfigureAwait(false);
                _scryFallCardTransformer = new ScryFallCardTransformer(DownloadManager, DownloadReporter);
                _scryFallCardTransformer.Finished += ScryFallCardTransformerFinished;

                CountDown = cards.Length;
                DownloadReporter.Total = cards.Length;
                _scryFallCardTransformer.AddRange(cards);
                _scryFallCardTransformer.StartLanguage();
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
                JobFinished();
            }
        }
        private void ScryFallCardTransformerFinished(object sender, EventArgs e)
        {
            _scryFallCardTransformer.Finished -= ScryFallCardTransformerFinished;
            _scryFallCardTransformer.Error -= ScryFallCardTransformerError;
            _scryFallCardTransformer = null;

            JobFinished();
        }
        private void ScryFallCardTransformerError(object sender, EventArgs<string> e)
        {
            AppendMessage(e.Data, false);
        }
        protected override void OnStopRequested()
        {
            _scryFallCardTransformer?.Stop();
        }
    }
}