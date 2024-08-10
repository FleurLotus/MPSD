namespace MagicPictureSetDownloader.ViewModel.Download.Edition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;

    using Common.Library.Collection;
    using Common.Library.Notify;
    using Common.ViewModel;
    using Common.Web;

    using MagicPictureSetDownloader.Core;

    public class DownloadEditionViewModel : DownloadViewModelBase
    {
        public event EventHandler<EventArgs<NewEditionInfoViewModel>> NewEditionCreated;

        private bool _disposed;
        private bool _hasJob;
        private DownloadManagerEdition _downloadManagerEdition;

        public DownloadEditionViewModel()
            : base("Download new editions")
        {
            DownloadManager.NewEditionCreated += OnNewEditionCreated;
            Editions = new AsyncObservableCollection<EditionInfoViewModel>();
            FeedEditionsCommand = new RelayCommand(FeedEditionsCommandExecute, FeedEditionsCommandCanExecute);
        }

        public IList<EditionInfoViewModel> Editions { get; }
        public ICommand FeedEditionsCommand { get; }
        public bool HasJob
        {
            get { return _hasJob; }
            set
            {
                if (value != _hasJob)
                {
                    _hasJob = value;
                    OnNotifyPropertyChanged(nameof(HasJob));
                }
            }
        }

        #region Command

        private bool FeedEditionsCommandCanExecute(object o)
        {
            return !IsBusy && HasJob;
        }
        private void FeedEditionsCommandExecute(object o)
        {
            JobStarting();
            ThreadPool.QueueUserWorkItem(FeedEditionsCallBack);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (DownloadManager != null)
                {
                    DownloadManager.NewEditionCreated -= OnNewEditionCreated;
                }
            }
            _disposed = true;

            base.Dispose(disposing);
        }

        protected override bool StartImpl()
        {
            ThreadPool.QueueUserWorkItem(GetEditionListCallBack, null);
            return true;
        }

        private void GetEditionListCallBack(object state)
        {
            try
            {
                HasJob = false;

                while (Editions.Count > 0)
                {
                    EditionInfoViewModel editionInfoViewModel = Editions[0];
                    editionInfoViewModel.PropertyChanged -= EditionInfoViewModelPropertyChanged;
                    Editions.Remove(editionInfoViewModel);
                }

                foreach (EditionInfoWithBlock editionInfoWithBlock in DownloadManager.GetEditionList(DownloadManager.BaseEditionUrl).Where(s => !s.Edition.Completed))
                {
                    EditionInfoViewModel editionInfoViewModel = new EditionInfoViewModel(DownloadManager.BaseEditionUrl, editionInfoWithBlock);
                    editionInfoViewModel.PropertyChanged += EditionInfoViewModelPropertyChanged;
                    Editions.Add(editionInfoViewModel);
                }
                if (Editions.Count == 0)
                {
                    SetMessage("Not any new edition");
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
            }
            JobFinished();
        }
        private void FeedEditionsCallBack(object state)
        {
            DownloadReporter.Reset();
            _downloadManagerEdition = new DownloadManagerEdition(DownloadManager, DownloadReporter);
            _downloadManagerEdition.Finished += DownloadManagerEditionFinished;
            _downloadManagerEdition.Error += DownloadManagerEditionError;

            foreach (EditionInfoViewModel editionInfoViewModel in Editions.Where(s => s.Active))
            {
                Interlocked.Increment(ref CountDown);
                string[] cardInfos = DownloadManager.GetCardUrls(editionInfoViewModel.Url);

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
        private void EditionInfoViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Active")
            {
                HasJob = Editions.Any(sivm => sivm.Active);
            }
        }
        private void OnNewEditionCreated(object sender, EventArgs<string> args)
        {
            var e = NewEditionCreated;
            if (e != null)
            {
                string name = args.Data;
                NewEditionInfoViewModel vm = new NewEditionInfoViewModel(name, DownloadManager.GetEditionIcon);
                DispatcherInvoker.Invoke(() => e(sender, new EventArgs<NewEditionInfoViewModel>(vm)));
            }
        }
    }
}
