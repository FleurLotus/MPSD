namespace MagicPictureSetDownloader.ViewModel.Download.Auto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AutoDownloadViewModelBase : DownloadViewModelBase
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private IReadOnlyList<(string url, object param)> _urls;
        private int _nextJob;
        private volatile bool _fatalException;
        private volatile bool _isCancelled;
        private int _finishCalled;
        private const int NbThread = 5;
        private readonly ManualResetEvent _firstDoneEvent = new ManualResetEvent(false);

        protected AutoDownloadViewModelBase(string title)
            : base(title)
        {
        }

        protected abstract IAsyncEnumerable<(string url, object param)> GetUrls(CancellationToken ct);
        protected abstract Task<string> Download(string url, object param, CancellationToken ct);

        protected override async Task<bool> StartImpl(CancellationToken ct)
        {
            _urls = await GetUrls(ct).ToArrayAsync(ct).ConfigureAwait(true);
            CountDown = _urls.Count;
            DownloadReporter.Total = CountDown;
            _finishCalled = 0;

            if (CountDown == 0)
            {
                SetMessage("Not any data to download");
                return false;
            }

            for (int i = 0; i < NbThread; i++)
            {
                _ = Task.Run(() => Downloader(ct), ct);
            }

            return true;
        }
        private async Task Downloader(CancellationToken ct)
        {
            string url = null;
            object param;

            while (true)
            {
                int currentJob = -1;
                try
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        currentJob = _nextJob;
                        _nextJob++;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }

                    if (currentJob >= _urls.Count || IsStopping)
                    {
                        break;
                    }

                    (url, param) = _urls[currentJob];

                    if (currentJob != 0)
                    {
                        int signaled = WaitHandle.WaitAny(new WaitHandle[] { _firstDoneEvent, ct.WaitHandle });
                        if (signaled == 1) // cancellation requested
                        {
                            break;
                        }
                    }

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    if (_fatalException)
                    {
                        break;
                    }

                    string errors = await Download(url, param, ct).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        AppendMessage(string.Format("{0} -> {1}", url, errors), false);
                    }
                    DownloadReporter.Progress();
                }
                catch (OperationCanceledException)
                {
                    // Cancellation requested -> exit worker loop
                    _isCancelled = true;
                    break;
                }
                catch (Exception ex)
                {
                    if (currentJob == 0)
                    {
                        _fatalException = true;
                    }
                    string errormessage = ex.InnerException?.Message ?? ex.Message;
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

                int newcount = Interlocked.Decrement(ref CountDown);
                if ((newcount == 0 || IsStopping || _fatalException || _isCancelled) && Interlocked.CompareExchange(ref _finishCalled, 1, 0) == 0)
                {
                    DownloadReporter.Finish();
                    JobFinished();
                }
            }
        }
    }
}