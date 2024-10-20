﻿namespace MagicPictureSetDownloader.UI
{
    using System;
    using System.Windows;

    using Common.Library.Notify;
    using Common.Web;
    using Common.ViewModel.Web;
    using Common.WPF;
    using Common.WPF.UI;

    using MagicPictureSetDownloader.ViewModel.Download;

    /// <summary>
    /// Interaction logic for DownloadImageWindow.xaml
    /// </summary>
    public partial class AutoDownloadWindow
    {
        public AutoDownloadWindow(DownloadViewModelBase vm)
        {
            DataContext = vm;
            InitializeComponent();
            vm.Start(new DispatcherInvoker(Application.Current.Dispatcher));
        }
        public void CredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            CredentialInputViewModel vm = new CredentialInputViewModel();
            CommonDialog f = new CommonDialog(vm) { Owner = this, WindowStyle = WindowStyle.ToolWindow };
            f.ShowDialog();
            if (vm.Result == true)
            {
                args.Data.Login = vm.Login;
                args.Data.Password = vm.Password;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnClosed(e);
        }
    }
}
