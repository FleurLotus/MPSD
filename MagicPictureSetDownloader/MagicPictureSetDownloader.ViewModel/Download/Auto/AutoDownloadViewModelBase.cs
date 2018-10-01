﻿namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System;
    using System.Threading;

    public abstract class AutoDownloadViewModelBase : DownloadViewModelBase
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly string[] _urls;
        private int _nextJob;
        private volatile bool _fatalException;
        private const int NbThread = 5;
        private readonly ManualResetEvent _firstDoneEvent = new ManualResetEvent(false);

        protected AutoDownloadViewModelBase(string title)
            : base(title)
        {
            _urls = GetUrls();
            CountDown = _urls.Length;
            DownloadReporter.Total = CountDown;
        }

        protected abstract string[] GetUrls();
        protected abstract void Download(string url);

        protected override bool StartImpl()
        {
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
            while (true)
            {
                int currentJob = -1;
                try
                {
                    _lock.EnterWriteLock();
                    currentJob = _nextJob;
                    _nextJob++;
                    _lock.ExitWriteLock();

                    if (currentJob >= _urls.Length || IsStopping)
                    {
                        break;
                    }

                    //Do the first alone to wait for proxy if needed
                    if (currentJob == 0 || _firstDoneEvent.WaitOne())
                    {
                        if (_fatalException)
                        {
                            break;
                        }

                        Download(_urls[currentJob]);
                    }
                    DownloadReporter.Progress();

                }
                catch (Exception ex)
                {
                    if (currentJob == 0)
                    {
                        _fatalException = true;
                    }

                    AppendMessage(string.Format("{0} -> {1}", _urls[currentJob], ex.Message), false);
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