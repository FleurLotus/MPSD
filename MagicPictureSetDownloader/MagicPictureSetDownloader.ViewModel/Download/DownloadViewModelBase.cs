namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Windows;

    using Common.Library;
    using Common.Library.Notify;
    using Common.ViewModel;
    using Common.Web;

    using MagicPictureSetDownloader.Core;

    public abstract class DownloadViewModelBase : NotifyPropertyChangedBase, IDisposable
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        protected readonly DownloadManager DownloadManager;
        protected IDispatcherInvoker DispatcherInvoker;

        protected int CountDown;
        protected bool IsStopping;
        protected readonly ManualResetEvent FinishedStopping = new ManualResetEvent(true);
        private bool _disposed;
        private bool _isBusy;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        
        protected DownloadViewModelBase(string title)
        {
            Title = title;
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


        public string Title { get; private set; }
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
                throw new ArgumentNullException("dispatcherInvoker");
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
