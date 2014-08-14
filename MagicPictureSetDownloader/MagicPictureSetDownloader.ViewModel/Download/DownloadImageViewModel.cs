namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Threading;

    using Common.Libray;

    public class DownloadImageViewModel : DownloadViewModelBase
    {
        private readonly string[] _missingImages;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private int _nextJob = 0;
        private const int NbThread = 5;
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);


        public DownloadImageViewModel(IDispatcherInvoker dispatcherInvoker)
            : base(dispatcherInvoker)
        {

            _missingImages = DownloadManager.GetMissingPictureUrls();
            CountDown = _missingImages.Length;
            DownloadReporter.Total = CountDown;
            JobStarting();
            for (int i = 0; i < NbThread; i++)
            {
                ThreadPool.QueueUserWorkItem(Downloader);
            }
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

                    if (currentJob >= _missingImages.Length)
                        break;

                    if (currentJob == 0 || _manualResetEvent.WaitOne())
                    {
                        DownloadManager.InsertPictureInDb(_missingImages[currentJob]);
                    }
                    DownloadReporter.Progress();

                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                }
                finally
                {
                    if (currentJob == 0)
                        _manualResetEvent.Set();
                }

                Interlocked.Decrement(ref CountDown);
            }

            if (CountDown == 0)
            {
                DownloadReporter.Finish();
                JobFinished();
            }
        }
    }
}
