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
    using MagicPictureSetDownloader.ViewModel.Download.Edition;

    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow
    {
        public DownloadWindow(DownloadViewModelBase vm)
        {
            DataContext = vm;
            InitializeComponent();
            vm.Start(new DispatcherInvoker(Application.Current.Dispatcher) );
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
        public void NewEditionCreated(object sender, EventArgs<NewEditionInfoViewModel> args)
        {
            NewEditionInfoViewModel vm = args.Data;
            CommonDialog f = new CommonDialog(vm) { Owner = this, WindowStyle = WindowStyle.ToolWindow }; 
            f.ShowDialog();
            if (vm.Result == true)
            {
                vm.Save();
            }
            else
            {
                vm.SaveDefault();
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
