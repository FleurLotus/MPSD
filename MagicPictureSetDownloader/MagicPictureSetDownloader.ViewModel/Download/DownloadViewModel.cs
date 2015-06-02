namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;

    using Common.Libray;
    using Common.Libray.Collection;
    using Common.Libray.Notify;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Core.EditionInfos;

    public class DownloadViewModel : DownloadViewModelBase
    {
        public event EventHandler<EventArgs<NewEditionInfoViewModel>> NewEditionCreated;

        private readonly IDictionary<IconPageType, string> _baseEditionIconUrls;
        private bool _disposed;
        private string _baseEditionUrl;
        private bool _hasJob;
        
        public DownloadViewModel(IDispatcherInvoker dispatcherInvoker, bool showConfig)
            : base(dispatcherInvoker)
        {
            BaseEditionUrl = @"http://gatherer.wizards.com/Pages/Default.aspx";

            _baseEditionIconUrls = new Dictionary<IconPageType, string>
                                       {
                                           { IconPageType.Wikia, @"http://mtg.wikia.com/wiki/Set" },
                                           { IconPageType.CardKingdom, @"http://www.cardkingdom.com/catalog" }
                                       };

            DownloadManager.NewEditionCreated += OnNewEditionCreated;
            ShowConfig = showConfig;
            Editions = new AsyncObservableCollection<EditionInfoViewModel>();
            GetEditionListCommand = new RelayCommand(GetEditionListCommandExecute, GetEditionListCommandCanExecute);
            FeedEditionsCommand = new RelayCommand(FeedEditionsCommandExecute, FeedEditionsCommandCanExecute);

            if (!ShowConfig)
                GetEditionListCommand.Execute(null);
        }

        public IList<EditionInfoViewModel> Editions { get; private set; }
        public ICommand GetEditionListCommand { get; private set; }
        public ICommand FeedEditionsCommand { get; private set; }
        public bool ShowConfig { get; private set; }
        public bool HasJob
        {
            get { return _hasJob; }
            set
            {
                if (value != _hasJob)
                {
                    _hasJob = value;
                    OnNotifyPropertyChanged(() => HasJob);
                }
            }
        }
        public string BaseEditionUrl
        {
            get { return _baseEditionUrl; }
            set
            {
                if (value != _baseEditionUrl)
                {
                    _baseEditionUrl = value;
                    OnNotifyPropertyChanged(() => BaseEditionUrl);
                }
            }
        }

        #region Command

        private bool GetEditionListCommandCanExecute(object o)
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(BaseEditionUrl);
        }
        private void GetEditionListCommandExecute(object o)
        {
            JobStarting();
            ThreadPool.QueueUserWorkItem(GetEditionListCallBack, BaseEditionUrl);
        }

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
                return;

            if (disposing)
            {
                if (DownloadManager != null)
                    DownloadManager.NewEditionCreated += OnNewEditionCreated;
            }
            _disposed = true;

            base.Dispose(disposing);
        }
        
        private void GetEditionListCallBack(object state)
        {
            try
            {
                string baseUrl = (string)state;
                HasJob = false;

                while (Editions.Count > 0)
                {
                    EditionInfoViewModel editionInfoViewModel = Editions[0];
                    editionInfoViewModel.PropertyChanged -= EditionInfoViewModelPropertyChanged;
                    Editions.Remove(editionInfoViewModel);
                }

                foreach (EditionInfoWithBlock editionInfoWithBlock in DownloadManager.GetEditionList(baseUrl).Where(s=> !s.Edition.Completed))
                {
                    EditionInfoViewModel editionInfoViewModel = new EditionInfoViewModel(BaseEditionUrl, editionInfoWithBlock);
                    editionInfoViewModel.PropertyChanged += EditionInfoViewModelPropertyChanged;
                    Editions.Add(editionInfoViewModel);
                }
                if (Editions.Count == 0)
                    SetMessage("Not any new edition");
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
                ThreadPool.QueueUserWorkItem(RetrieveEditionDataCallBack, new object[] { model.DownloadReporter, model.EditionId, cardInfos.Select(s => DownloadManager.ToAbsoluteUrl(model.Url, s)) });
            }
        }
        private void RetrieveEditionDataCallBack(object state)
        {
            object[] args = (object[])state;

            DownloadReporterViewModel editionDownloadReporter = (DownloadReporterViewModel)args[0];
            int editionId = (int)args[1];
            IEnumerable<string> urls = (IEnumerable<string>)args[2];

            try
            {
                bool hasCard = false;
                foreach (string cardUrl in urls)
                {
                    if (IsStopping)
                        break;
                    DownloadManager.GetCardInfo(cardUrl, editionId);
                    hasCard = true;
                    editionDownloadReporter.Progress();
                    DownloadReporter.Progress();
                }
                if (!IsStopping && hasCard)
                    DownloadManager.EditionCompleted(editionId);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message, false);
            }

            editionDownloadReporter.Finish();
            Interlocked.Decrement(ref CountDown);
            if (CountDown == 0)
            {
                DownloadReporter.Finish();
                
                //Keep previous error
                string msg = Message;
                GetEditionListCommand.Execute(null);

                if (!string.IsNullOrWhiteSpace(msg))
                    AppendMessage(msg, true);
                JobFinished();
            }
        }
        private void EditionInfoViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Active")
                HasJob = Editions.Any(sivm => sivm.Active);
        }
        private void OnNewEditionCreated(object sender, EventArgs<string> args)
        {
            var e = NewEditionCreated;
            if (e != null)
            {
                string name = args.Data;
                NewEditionInfoViewModel vm = new NewEditionInfoViewModel(name, DownloadManager.GetEditionIcon(_baseEditionIconUrls, name));
                DispatcherInvoker.Invoke(() => e(sender, new EventArgs<NewEditionInfoViewModel>(vm)));
            }
        }
    }
}
