using System;
using System.IO;
using System.Windows.Input;
using CommonInterface;
using CommonViewModel;

namespace MagicPictureSetDownloader.ViewModel
{
    public class MainViewModel: NotifyPropertyChangedBase
    {
        
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private string _baseSetUrl;
        private string _outputPath;
        private bool _isBusy;
        private readonly DownloadManager _downloadManager;
        private readonly IDispatcherInvoker _dispatcherInvoker;
        
        public MainViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            _dispatcherInvoker = dispatcherInvoker;
            BaseSetUrl = @"http://mythicspoiler.com/sets.html";
            RunCommand = new RelayCommand(RunCommandExecute, RunCommandCanExecute);
            _downloadManager = new DownloadManager();
        }

        public ICommand RunCommand { get; private set; }
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
        

        #region Command
        private bool RunCommandCanExecute(object o)
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(BaseSetUrl) && Directory.Exists(OutputPath);
        }
        private void RunCommandExecute(object o)
        {
            IsBusy = true;
            _downloadManager.RunCompleted += DownloadManagerRunCompleted;
            _downloadManager.RunError += DownloadManagerRunError;
            _downloadManager.CredentialRequiered += OnCredentialRequiered;
            _downloadManager.RunAsync(new DownloadManagerRunArgs(BaseSetUrl, OutputPath));
        }
        private void OnCredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            var e = CredentialRequiered;
            if (e != null)
                _dispatcherInvoker.Invoke(() => e(sender, args));
        }
        private void DownloadManagerRunCompleted(object sender, EventArgs<string> e)
        {
            throw new NotImplementedException();
        }
        private void DownloadManagerRunError(object sender, EventArgs<Exception> e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
