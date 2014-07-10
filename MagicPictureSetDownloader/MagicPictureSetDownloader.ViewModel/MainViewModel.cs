using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Common.Libray;
using Common.ViewModel;
using MagicPictureSetDownloader.Core;

namespace MagicPictureSetDownloader.ViewModel
{
    public class MainViewModel: NotifyPropertyChangedBase
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private string _baseSetUrl;
        private string _message;
        private bool _isBusy;
        private bool _hasJob;
        private int _countDown;
        private readonly DownloadManager _downloadManager;
        private readonly IDispatcherInvoker _dispatcherInvoker;
        
        public MainViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            _dispatcherInvoker = dispatcherInvoker;
            BaseSetUrl = @"http://gatherer.wizards.com/Pages/Default.aspx";
            
            Sets = new AsyncObservableCollection<SetInfoViewModel>();
            GetSetListCommand = new RelayCommand(GetSetListCommandExecute, GetSetListCommandCanExecute);
            FeedSetsCommand = new RelayCommand(FeedSetsCommandExecute, FeedSetsCommandCanExecute);
            DownloadReporter = new DownloadReporter();
            _downloadManager = new DownloadManager();
            _downloadManager.CredentialRequiered += OnCredentialRequiered;
        }

        public AsyncObservableCollection<SetInfoViewModel> Sets { get; private set; }
        public ICommand GetSetListCommand { get; private set; }
        public ICommand FeedSetsCommand { get; private set; }
        public DownloadReporter DownloadReporter { get; private set; }
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    OnNotifyPropertyChanged(() => IsBusy);
                }
            }
        }
        public bool HasJob
        {
            get
            {
                return _hasJob;
            }
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
            get
            {
                return _baseSetUrl;
            }
            set
            {
                if (value != _baseSetUrl)
                {
                    _baseSetUrl = value;
                    OnNotifyPropertyChanged(() => BaseSetUrl);
                }
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnNotifyPropertyChanged(() => Message);
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
                Sets.Clear();
                foreach (SetInfoWithBlock setInfoWithBlock in _downloadManager.GetSetList(baseUrl))
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
                Interlocked.Increment(ref _countDown);
                string[] cardInfos = _downloadManager.GetCardUrls(setInfoViewModel.Url);

                setInfoViewModel.DownloadReporter.Total = cardInfos.Length;
                DownloadReporter.Total += cardInfos.Length;

                SetInfoViewModel model = setInfoViewModel;
                ThreadPool.QueueUserWorkItem(RetrieveSetDataCallBack, new object[] {model.DownloadReporter, cardInfos.Select(s => DownloadManager.ToAbsoluteUrl(model.Url, s))});
            }
        }
        
        private void RetrieveSetDataCallBack(object state)
        {
            object[] args = (object[])state;
            DownloadReporter setDownloadReporter = (DownloadReporter)args[0];
            IEnumerable<string> urls = (IEnumerable<string>)args[1];
            
            foreach (string cardUrl in urls)
            {
                _downloadManager.GetCardInfo(cardUrl);
                setDownloadReporter.Progress();
                DownloadReporter.Progress();
            }
            
            setDownloadReporter.Finish();
            Interlocked.Decrement(ref _countDown);
            if (_countDown == 0)
            {
                DownloadReporter.Finish();
                IsBusy = false;
            }
        }
        
        private void SetInfoViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Active")
                HasJob = Sets.Any(sivm => sivm.Active);
        }

        private void OnCredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            var e = CredentialRequiered;
            if (e != null)
                _dispatcherInvoker.Invoke(() => e(sender, args));
        }
        private void JobStarting()
        {
            Message = null;
            IsBusy = true;
        }
        private void JobFinished()
        {
            IsBusy = false;
        }

    }
}
