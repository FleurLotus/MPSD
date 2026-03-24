namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Notify;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public class DownloadViewModel : DownloadViewModelBase
    {
        private bool _disposed;
        private ScryFallCardTransformer _scryFallCardTransformer;

        public DownloadViewModel()
            : base("Download new cards/editions")
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
                DownloadReporter.Total = 2;
                await DownloadManager.GetAndSaveEditions(ct).ConfigureAwait(false);
                DownloadReporter.Progress();
                Card[] cards = await DownloadManager.GetCards(false, ct).ToArrayAsync().ConfigureAwait(false);
                DownloadReporter.Progress();

                _scryFallCardTransformer = new ScryFallCardTransformer(DownloadManager, DownloadReporter);
                _scryFallCardTransformer.Finished += ScryFallCardTransformerFinished;
                _scryFallCardTransformer.Error += ScryFallCardTransformerError;

                DownloadReporter.Reset();
                CountDown = cards.Length;
                DownloadReporter.Total = cards.Length;
                _scryFallCardTransformer.AddRange(cards);
                _scryFallCardTransformer.Start();
            }
            catch (OperationCanceledException)
            {
                //No error, just stop the job
                JobFinished();
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