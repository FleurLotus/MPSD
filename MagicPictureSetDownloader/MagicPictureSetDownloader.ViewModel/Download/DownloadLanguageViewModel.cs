namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Threading;

    using Common.Library.Notify;

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

        protected override bool StartImpl()
        {
            ThreadPool.QueueUserWorkItem(GetJsonData, null);
            return true;
        }

        private void GetJsonData(object state)
        {
            try
            {
                Card[] cards = DownloadManager.GetAllCards();
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
