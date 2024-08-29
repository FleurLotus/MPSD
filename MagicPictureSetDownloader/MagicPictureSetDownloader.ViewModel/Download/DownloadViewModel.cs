namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Threading;

    using Common.Library.Notify;

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

        protected override bool StartImpl()
        {
            ThreadPool.QueueUserWorkItem(GetJsonData, null);
            return true;
        }

        private void GetJsonData(object state)
        {
            try
            {
                DownloadManager.GetAndSaveEditions();

                Card[] cards = DownloadManager.GetCards();
                _scryFallCardTransformer = new ScryFallCardTransformer(DownloadManager, DownloadReporter);
                _scryFallCardTransformer.Finished += ScryFallCardTransformerFinished;
                _scryFallCardTransformer.Error += ScryFallCardTransformerError;

                CountDown = cards.Length;
                DownloadReporter.Total = cards.Length;
                _scryFallCardTransformer.AddRange(cards);
                _scryFallCardTransformer.Start();
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
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
