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
    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;

    public class DownloadViewModel : DownloadViewModelBase
    {
        private string _baseSetUrl;
        private bool _hasJob;
        
        public DownloadViewModel(IDispatcherInvoker dispatcherInvoker, bool showConfig)
            : base(dispatcherInvoker)
        {
            BaseSetUrl = @"http://gatherer.wizards.com/Pages/Default.aspx";

            ShowConfig = showConfig;
            Sets = new AsyncObservableCollection<SetInfoViewModel>();
            GetSetListCommand = new RelayCommand(GetSetListCommandExecute, GetSetListCommandCanExecute);
            FeedSetsCommand = new RelayCommand(FeedSetsCommandExecute, FeedSetsCommandCanExecute);

            if (!ShowConfig)
                GetSetListCommand.Execute(null);
        }

        public IList<SetInfoViewModel> Sets { get; private set; }
        public ICommand GetSetListCommand { get; private set; }
        public ICommand FeedSetsCommand { get; private set; }
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
        public string BaseSetUrl
        {
            get { return _baseSetUrl; }
            set
            {
                if (value != _baseSetUrl)
                {
                    _baseSetUrl = value;
                    OnNotifyPropertyChanged(() => BaseSetUrl);
                }
            }
        }

        #region Command

        private bool GetSetListCommandCanExecute(object o)
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(BaseSetUrl);
        }
        private void GetSetListCommandExecute(object o)
        {
            JobStarting();
            ThreadPool.QueueUserWorkItem(GetSetListCallBack, BaseSetUrl);
        }

        private bool FeedSetsCommandCanExecute(object o)
        {
            return !IsBusy && HasJob;
        }
        private void FeedSetsCommandExecute(object o)
        {
            JobStarting();
            ThreadPool.QueueUserWorkItem(FeedSetsCallBack);
        }

        #endregion

        private void GetSetListCallBack(object state)
        {
            try
            {
                string baseUrl = (string)state;
                HasJob = false;

                while (Sets.Count > 0)
                {
                    SetInfoViewModel setInfoViewModel = Sets[0];
                    setInfoViewModel.PropertyChanged -= SetInfoViewModelPropertyChanged;
                    Sets.Remove(setInfoViewModel);
                }

                foreach (SetInfoWithBlock setInfoWithBlock in DownloadManager.GetSetList(baseUrl).Where(s=> !s.Edition.Completed))
                {
                    SetInfoViewModel setInfoViewModel = new SetInfoViewModel(BaseSetUrl, setInfoWithBlock);
                    setInfoViewModel.PropertyChanged += SetInfoViewModelPropertyChanged;
                    Sets.Add(setInfoViewModel);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            JobFinished();
        }
        private void FeedSetsCallBack(object state)
        {
            DownloadReporter.Reset();

            foreach (SetInfoViewModel setInfoViewModel in Sets.Where(s => s.Active))
            {
                Interlocked.Increment(ref CountDown);
                string[] cardInfos = DownloadManager.GetCardUrls(setInfoViewModel.Url);

                setInfoViewModel.DownloadReporter.Total = cardInfos.Length;
                DownloadReporter.Total += cardInfos.Length;

                SetInfoViewModel model = setInfoViewModel;
                ThreadPool.QueueUserWorkItem(RetrieveSetDataCallBack, new object[] { model.DownloadReporter, model.EditionId, cardInfos.Select(s => DownloadManager.ToAbsoluteUrl(model.Url, s)) });
            }
        }
        private void RetrieveSetDataCallBack(object state)
        {
            object[] args = (object[])state;
            DownloadReporterViewModel setDownloadReporter = (DownloadReporterViewModel)args[0];
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
                    setDownloadReporter.Progress();
                    DownloadReporter.Progress();
                }
                if (!IsStopping && hasCard)
                    DownloadManager.EditionCompleted(editionId);
            }
            catch (Exception ex)
            {
                Message += ex.Message;
            }

            setDownloadReporter.Finish();
            Interlocked.Decrement(ref CountDown);
            if (CountDown == 0)
            {
                DownloadReporter.Finish();
                
                //Keep previous error
                string msg = Message;
                GetSetListCommand.Execute(null);
                
                if (!string.IsNullOrWhiteSpace(msg))
                    Message = msg + Message;
                JobFinished();
            }
        }
        private void SetInfoViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Active")
                HasJob = Sets.Any(sivm => sivm.Active);
        }
    }
}
