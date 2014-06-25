using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Input;
using Common.Interface;
using Common.ViewModel;
using CommonLibray;
using MagicPictureSetDownloader.Core;

namespace MagicPictureSetDownloader.ViewModel
{
    public class MainViewModel: NotifyPropertyChangedBase
    {
  
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private string _baseSetUrl;
        private string _outputPath;
        private string _message;
        private bool _isBusy;
        private int _countDown;
        private readonly DownloadManager _downloadManager;
        private readonly IDispatcherInvoker _dispatcherInvoker;
        
        public MainViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            _dispatcherInvoker = dispatcherInvoker;
            BaseSetUrl = @"http://mythicspoiler.com/sets.html";
            Sets = new AsyncObservableCollection<SetInfoViewModel>();
            GetSetListCommand = new RelayCommand(GetSetListCommandExecute, GetSetListCommandCanExecute);
            GetPicturesCommand = new RelayCommand(GetPicturesCommandExecute, GetPicturesCommandCanExecute);
            DownloadReporter = new DownloadReporter();
            _downloadManager = new DownloadManager();
            _downloadManager.CredentialRequiered += OnCredentialRequiered;
        }

        public AsyncObservableCollection<SetInfoViewModel> Sets { get; private set; }
        public ICommand GetSetListCommand { get; private set; }
        public ICommand GetPicturesCommand { get; private set; }
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
        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                if (value != _outputPath)
                {
                    _outputPath = value;
                    OnNotifyPropertyChanged(() => OutputPath);
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
        private bool GetPicturesCommandCanExecute(object o)
        {
            return !IsBusy && Directory.Exists(OutputPath);
        }
        private void GetPicturesCommandExecute(object o)
        {
            JobStarting();
            ThreadPool.QueueUserWorkItem(GetPicturesListCallBack);
        }

        #endregion
        private void GetSetListCallBack(object state)
        {
            try
            {
                string baseUrl = (string)state;
                Sets.Clear();
                foreach (SetInfo setInfo in _downloadManager.GetSetList(baseUrl))
                {
                    SetInfoViewModel setInfoViewModel = new SetInfoViewModel(BaseSetUrl, setInfo);
                    Sets.Add(setInfoViewModel);
                    ThreadPool.QueueUserWorkItem(GetNameCallback, setInfoViewModel);
                }
            }
            catch (Exception ex)
            {

                Message = ex.Message;
            }
            JobFinished();
        }
        private void GetNameCallback(object state)
        {
            try
            {
                SetInfoViewModel setInfoViewModel = (SetInfoViewModel)state;
                setInfoViewModel.Name = _downloadManager.GetName(setInfoViewModel.Url);
            }
            catch (WebException)
            {
            }
        }
        private void GetPicturesListCallBack(object state)
        {
            foreach (SetInfoViewModel setInfoViewModel in Sets.Where(s => s.Active))
            {
                Interlocked.Increment(ref _countDown);
                PictureInfo[] pictures = _downloadManager.GetPicturesList(setInfoViewModel.Url);

                string alias = setInfoViewModel.Alias;
                if (alias == "con")
                    alias = "cns";
                string outpath = Path.Combine(OutputPath, alias);
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                setInfoViewModel.DownloadReporter.Total = pictures.Length;
                DownloadReporter.Total += pictures.Length;
                ThreadPool.QueueUserWorkItem(GetPicturesForOneSetCallBack, new object[] {outpath,setInfoViewModel.Url, pictures, setInfoViewModel.DownloadReporter});
            }
        }
        private void GetPicturesForOneSetCallBack(object state)
        {
            object[] args = (object[])state;
            string output = (string)args[0];
            string baseUrl = (string)args[1];
            PictureInfo[] pictures = (PictureInfo[])args[2];
            DownloadReporter setDownloadReporter = (DownloadReporter)args[3];

            foreach (PictureInfo pictureInfo in pictures)
            {
                string nameurl = DownloadManager.ToAbsoluteUrl(baseUrl, pictureInfo.Url);
                string pictureurl = DownloadManager.ToAbsoluteUrl(baseUrl, pictureInfo.PictureUrl);
                string name = _downloadManager.GetName(nameurl);
                _downloadManager.GetPicture(pictureurl, output, name);
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
