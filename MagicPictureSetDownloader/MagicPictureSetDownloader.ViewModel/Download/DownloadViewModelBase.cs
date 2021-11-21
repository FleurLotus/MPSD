namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Text;
    using System.Threading;

    using Common.Library;
    using Common.Library.Notify;
    using Common.ViewModel;
    using Common.Web;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;

    public abstract class DownloadViewModelBase : NotifyPropertyChangedBase, IDisposable
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        protected readonly DownloadManager DownloadManager;
        protected IDispatcherInvoker DispatcherInvoker;

        protected int CountDown;
        protected bool IsStopping;
        protected readonly ManualResetEvent FinishedStopping = new ManualResetEvent(true);
        private bool _disposed;
        private IDisposable _batch;
        private readonly object _sync = new object();
        private bool _isBusy;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        
        protected DownloadViewModelBase(string title)
        {
            Title = title;
            DownloadReporter = new DownloadReporterViewModel();
            DownloadManager = new DownloadManager();
            DownloadManager.CredentialRequiered += OnCredentialRequiered;
        }

        public DownloadReporterViewModel DownloadReporter { get; }
        public string Title { get; }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    OnNotifyPropertyChanged(nameof(IsBusy));
                }
            }
        }
        public string Message
        {
            get
            {
                lock (_stringBuilder)
                {
                    return _stringBuilder.Length == 0 ? null : _stringBuilder.ToString();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Start(IDispatcherInvoker dispatcherInvoker)
        {
            if (null == dispatcherInvoker)
            {
                throw new ArgumentNullException(nameof(dispatcherInvoker));
            }

            DispatcherInvoker = dispatcherInvoker;

            JobStarting();
            if (!StartImpl())
            {
                JobFinished();
            }
        }

        protected abstract bool StartImpl();
        protected void JobStarting()
        {
            SetMessage(null);
            IsBusy = true;

            lock (_sync)
            {
                if (_batch == null)
                {
                    _batch = MagicDatabaseManager.ReadAndUpdate.BatchMode();
                }
            }

            FinishedStopping.Reset();
        }
        protected void JobFinished()
        {
            IsBusy = false;

            BatchEnd();

            if (!_disposed)
            {
                FinishedStopping.Set();
            }
        }

        private void BatchEnd()
        {
            lock (_sync)
            {
                if (_batch != null)
                {
                    _batch.Dispose();
                    _batch = null;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (DownloadManager!= null)
                {
                    DownloadManager.CredentialRequiered += OnCredentialRequiered;
                }

                IsStopping = true;
                OnStopRequested();
                FinishedStopping.WaitOne();
                FinishedStopping.Dispose();
            }
            _disposed = true;
        }
        protected virtual void OnStopRequested()
        {
        }
        protected void SetMessage(string msg)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(msg);
            }

            OnNotifyPropertyChanged(nameof(Message));
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
                    {
                        msg = "\n" + msg;
                    }

                    _stringBuilder.Append(msg);
                }
            }

            OnNotifyPropertyChanged(nameof(Message));
        }

        private void OnCredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            var e = CredentialRequiered;
            if (e != null)
            {
                DispatcherInvoker.Invoke(() => e(sender, args));
            }
        }
    }
}
