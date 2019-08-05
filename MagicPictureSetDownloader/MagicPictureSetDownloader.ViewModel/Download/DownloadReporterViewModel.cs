namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Threading;

    using Common.ViewModel;
    using MagicPictureSetDownloader.Interface;

    public class DownloadReporterViewModel : NotifyPropertyChangedBase, IDisposable, IProgressReporter
    {
        private bool _disposed;
        private int _current;
        private int _total;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public DownloadReporterViewModel()
        {
            Reset();
        }

        public int Total
        {
            get { return _total; }
            set
            {
                if (value != _total)
                {
                    _total = value;
                    OnNotifyPropertyChanged(nameof(Total));
                }
            }
        }

        public int Current
        {
            get
            {
                _lock.EnterReadLock();
                int ret = _current;
                _lock.ExitReadLock();
                return ret;
            }
        }
        public void Reset()
        {
            _lock.EnterWriteLock();
            _current = 0;
            _lock.ExitWriteLock();
            OnNotifyPropertyChanged(nameof(Current));

            Total = 1;
        }
        public void Progress()
        {
            _lock.EnterWriteLock();
            _current++;
            _lock.ExitWriteLock();
            OnNotifyPropertyChanged(nameof(Current));
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Finish()
        {
            _lock.EnterWriteLock();
            _current = _total;
            _lock.ExitWriteLock();
            OnNotifyPropertyChanged(nameof(Current));

        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_lock != null)
                {
                    _lock.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
