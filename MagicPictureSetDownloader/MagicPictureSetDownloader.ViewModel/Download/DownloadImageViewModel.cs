namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Threading;

    using Common.Libray;

    public class DownloadImageViewModel : DownloadViewModelBase
    {
        private readonly string[] _missingImages;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private int _nextJob;
        private volatile bool _fatalException;
        private const int NbThread = 5;
        private readonly ManualResetEvent _firstDoneEvent = new ManualResetEvent(false);


        public DownloadImageViewModel(IDispatcherInvoker dispatcherInvoker)
            : base(dispatcherInvoker)
        {

            _missingImages = DownloadManager.GetMissingPictureUrls();
            CountDown = _missingImages.Length;
            DownloadReporter.Total = CountDown;

            if (CountDown == 0)
            {
                SetMessage("Not any image to download");
            }

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

                    if (currentJob >= _missingImages.Length || IsStopping)
                        break;

                    //Do the first alone to wait for proxy if needed
                    if (currentJob == 0 || _firstDoneEvent.WaitOne())
                    {
                        if (_fatalException)
                            break;

                        DownloadManager.InsertPictureInDb(_missingImages[currentJob]);
                    }
                    DownloadReporter.Progress();

                }
                catch (Exception ex)
                {
                    if (currentJob == 0)
                        _fatalException = true;

                    AppendMessage(ex.Message, false);
                }
                finally
                {
                    //First finished, Go for the others
                    if (currentJob == 0)
                        _firstDoneEvent.Set();
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
