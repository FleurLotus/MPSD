﻿namespace MagicPictureSetDownloader.UI
{
    using System;

    using Common.Libray;
    using Common.WPF;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.ViewModel.Download;

    /// <summary>
    /// Interaction logic for DownloadImageWindow.xaml
    /// </summary>
    public partial class DownloadImageWindow
    {
        public DownloadImageWindow()
        {
            DataContext = new DownloadImageViewModel(new DispatcherInvoker());
            InitializeComponent();
        }
        public void CredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            CredentialInputViewModel vm = new CredentialInputViewModel();
            CredentialInputWindow f = new CredentialInputWindow(vm) { Owner = this };
            f.ShowDialog();
            if (vm.Result.HasValue && vm.Result.Value)
            {
                args.Data.Login = vm.Login;
                args.Data.Password = vm.Password;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            IDisposable disposable = DataContext as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            base.OnClosed(e);
        }
    }
}