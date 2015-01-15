namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Threading;

    using Common.Libray;
    using Common.Libray.Notify;
    using Common.ViewModel;
    using MagicPictureSetDownloader.Core;

    public class DownloadViewModelBase : NotifyPropertyChangedBase, IDisposable
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;
        public event EventHandler<EventArgs<string>> NewEditionCreated;

        protected readonly IDispatcherInvoker DispatcherInvoker;
        protected readonly DownloadManager DownloadManager;

        protected int CountDown;
        protected bool IsStopping;
        protected readonly ManualResetEvent FinishedStopping = new ManualResetEvent(true);
        private bool _disposed;
        private bool _isBusy;
        private string _message;
        
        public DownloadViewModelBase(IDispatcherInvoker dispatcherInvoker)
        {
            DispatcherInvoker = dispatcherInvoker;
            DownloadReporter = new DownloadReporterViewModel();
            DownloadManager = new DownloadManager();
            DownloadManager.CredentialRequiered += OnCredentialRequiered;
            DownloadManager.NewEditionCreated += OnNewEditionCreated;
        }

        public DownloadReporterViewModel DownloadReporter { get; private set; }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    OnNotifyPropertyChanged(() => IsBusy);
                }
            }
        }
        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnNotifyPropertyChanged(() => Message);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void JobStarting()
        {
            Message = null;
            IsBusy = true;
            FinishedStopping.Reset();
        }
        protected void JobFinished()
        {
            IsBusy = false;
            FinishedStopping.Set();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                IsStopping = true;
                FinishedStopping.WaitOne();
                FinishedStopping.Dispose();
            }
            _disposed = true;
        }
        
        private void OnCredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            var e = CredentialRequiered;
            if (e != null)
                DispatcherInvoker.Invoke(() => e(sender, args));
        }
        private void OnNewEditionCreated(object sender, EventArgs<string> args)
        {
            var e = NewEditionCreated;
            if (e != null)
                DispatcherInvoker.Invoke(() => e(sender, args));
        }
    }
}
