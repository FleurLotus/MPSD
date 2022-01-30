namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public abstract class AutoDownloadViewModelBase : DownloadViewModelBase
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private IReadOnlyList<KeyValuePair<string, object>> _urls;
        private int _nextJob;
        private volatile bool _fatalException;
        private const int NbThread = 5;
        private readonly ManualResetEvent _firstDoneEvent = new ManualResetEvent(false);

        protected AutoDownloadViewModelBase(string title)
            : base(title)
        {
        }

        protected abstract IReadOnlyList<KeyValuePair<string, object>> GetUrls();
        protected abstract string Download(string url, object param);

        protected override bool StartImpl()
        {
            _urls = GetUrls();
            CountDown = _urls.Count;
            DownloadReporter.Total = CountDown;

            if (CountDown == 0)
            {
                SetMessage("Not any data to download");
                return false;
            }

            for (int i = 0; i < NbThread; i++)
            {
                ThreadPool.QueueUserWorkItem(Downloader);
            }

            return true;
        }
        private void Downloader(object state)
        {
            string url = null;

            while (true)
            {
                int currentJob = -1;
                try
                {
                    _lock.EnterWriteLock();
                    currentJob = _nextJob;
                    _nextJob++;
                    _lock.ExitWriteLock();

                    if (currentJob >= _urls.Count || IsStopping)
                    {
                        break;
                    }

                    KeyValuePair<string, object> kv = _urls[currentJob];
                    url = kv.Key;

                    //Do the first alone to wait for proxy if needed
                    if (currentJob == 0 || _firstDoneEvent.WaitOne())
                    {
                        if (_fatalException)
                        {
                            break;
                        }

                        string errors = Download(url, kv.Value);
                        if (!string.IsNullOrWhiteSpace(errors))
                        {
                            AppendMessage(string.Format("{0} -> {1}", url, errors), false);
                        }
                    }
                    DownloadReporter.Progress();
                }
                catch (Exception ex)
                {
                    if (currentJob == 0)
                    {
                        _fatalException = true;
                    }
                    string errormessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                    }

                    AppendMessage(string.Format("{0} -> {1}", url, errormessage), false);
                }
                finally
                {
                    //First finished, Go for the others
                    if (currentJob == 0)
                    {
                        _firstDoneEvent.Set();
                    }
                }

                Interlocked.Decrement(ref CountDown);
            }

            if (CountDown == 0 || IsStopping || _fatalException)
            {
                DownloadReporter.Finish();
                JobFinished();
            }
        }
    }
}