namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Text;
    using System.Threading;

    using Common.Libray;
    using Common.Libray.Notify;
    using Common.ViewModel;
    using MagicPictureSetDownloader.Core;

    public class DownloadViewModelBase : NotifyPropertyChangedBase, IDisposable
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        protected readonly IDispatcherInvoker DispatcherInvoker;
        protected readonly DownloadManager DownloadManager;

        protected int CountDown;
        protected bool IsStopping;
        protected readonly ManualResetEvent FinishedStopping = new ManualResetEvent(true);
        private bool _disposed;
        private bool _isBusy;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        
        public DownloadViewModelBase(IDispatcherInvoker dispatcherInvoker)
        {
            DispatcherInvoker = dispatcherInvoker;
            DownloadReporter = new DownloadReporterViewModel();
            DownloadManager = new DownloadManager();
            DownloadManager.CredentialRequiered += OnCredentialRequiered;
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
            get { return _stringBuilder.Length ==  0 ? null: _stringBuilder.ToString(); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void JobStarting()
        {
            SetMessage(null);
            IsBusy = true;
            FinishedStopping.Reset();
        }
        protected void JobFinished()
        {
            IsBusy = false;
            if (!_disposed)
                FinishedStopping.Set();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (DownloadManager!= null)
                    DownloadManager.CredentialRequiered += OnCredentialRequiered;
                IsStopping = true;
                FinishedStopping.WaitOne();
                FinishedStopping.Dispose();
            }
            _disposed = true;
        }


        protected void SetMessage(string msg)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(msg);
            }

            OnNotifyPropertyChanged(() => Message);
        }
        protected void AppendMessage(string msg, bool before)
        {
            lock (_stringBuilder)
            {
                if (before)
                {
                    _stringBuilder.Insert(0, msg);
                }
                else
                {
                    if (_stringBuilder.Length > 0)
                        msg = "\n" + msg;

                    _stringBuilder.Append(msg);
                }
            }

            OnNotifyPropertyChanged(() => Message);
        }

        private void OnCredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            var e = CredentialRequiered;
            if (e != null)
                DispatcherInvoker.Invoke(() => e(sender, args));
        }
    }
}
