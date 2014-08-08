namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using Common.Libray;
    using Common.ViewModel;
    using MagicPictureSetDownloader.Core;

    public class DownloadViewModelBase : NotifyPropertyChangedBase
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        protected readonly IDispatcherInvoker DispatcherInvoker;
        protected readonly DownloadManager DownloadManager;

        public DownloadViewModelBase(IDispatcherInvoker dispatcherInvoker)
        {
            DispatcherInvoker = dispatcherInvoker;
            DownloadReporter = new DownloadReporterViewModel();
            DownloadManager = new DownloadManager();
            DownloadManager.CredentialRequiered += OnCredentialRequiered;
        }

        public DownloadReporterViewModel DownloadReporter { get; private set; }

        private void OnCredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            var e = CredentialRequiered;
            if (e != null)
                DispatcherInvoker.Invoke(() => e(sender, args));
        }
    }
}
