namespace MagicPictureSetDownloader.ViewModel.Download.Edition
{
    using System;
    using System.Threading;

    using Common.Library.Notify;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public class DownloadViewModel : DownloadViewModelBase
    {
        private bool _disposed;
        private Card[] _cards;
        private DownloadManagerEdition _downloadManagerEdition;

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
                _cards = DownloadManager.GetCards();

                //ALERT read the data from the card to insert or update in ref, DownloadManagerEdition might become a new card transformer to validate 
                //ALERT mana cost '@'




            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
            }
        }
        private void FeedEditionsCallBack(object state)
        {
            /*
            DownloadReporter.Reset();
            _downloadManagerEdition = new DownloadManagerEdition(DownloadManager, DownloadReporter);
            _downloadManagerEdition.Finished += DownloadManagerEditionFinished;
            _downloadManagerEdition.Error += DownloadManagerEditionError;

            foreach (EditionInfoViewModel editionInfoViewModel in Editions.Where(s => s.Active))
            {
                Interlocked.Increment(ref CountDown);
                string[] cardInfos = null;

                editionInfoViewModel.DownloadReporter.Total = cardInfos.Length;
                DownloadReporter.Total += cardInfos.Length;

                EditionInfoViewModel model = editionInfoViewModel;
                if (model.CardNumber.HasValue)
                {
                    if (cardInfos.Length != model.CardNumber.Value)
                    {
                        AppendMessage(string.Format("{0}: {1} urls while cardnumber is set to {2}", model.Name, cardInfos.Length, model.CardNumber.Value), false);
                    }
                }

                _downloadManagerEdition.AddRange(cardInfos.Select(s => WebAccess.ToAbsoluteUrl(model.Url, s)), model.EditionId, model.DownloadReporter);
            }
            */
            _downloadManagerEdition.Start();
        }
        private void DownloadManagerEditionFinished(object sender, EventArgs e)
        {
            _downloadManagerEdition.Finished -= DownloadManagerEditionFinished;
            _downloadManagerEdition.Error -= DownloadManagerEditionError;
            _downloadManagerEdition = null;
            //Keep previous error
            string msg = Message;
            Start(DispatcherInvoker);

            if (!string.IsNullOrWhiteSpace(msg))
            {
                AppendMessage(msg, true);
            }

            JobFinished();
        }
        private void DownloadManagerEditionError(object sender, EventArgs<string> e)
        {
            AppendMessage(e.Data, false);
        }
        protected override void OnStopRequested()
        {
            _downloadManagerEdition?.Stop();
        }
    }
}
