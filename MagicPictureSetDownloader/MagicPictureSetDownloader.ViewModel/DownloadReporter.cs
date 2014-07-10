using System.Threading;
using Common.ViewModel;

namespace MagicPictureSetDownloader.ViewModel
{
    public class DownloadReporter : NotifyPropertyChangedBase
    {
        private int _current;
        private int _total;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public DownloadReporter()
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
                    OnNotifyPropertyChanged(() => Total);
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
            OnNotifyPropertyChanged(() => Current);

            Total = 1;
        }

        public void Progress()
        {
            _lock.EnterWriteLock();
            _current++;
            _lock.ExitWriteLock();
            OnNotifyPropertyChanged(() => Current);
        }

        internal void Finish()
        {
            _lock.EnterWriteLock();
            _current = _total;
            _lock.ExitWriteLock();
            OnNotifyPropertyChanged(() => Current);

        }
    }
}
