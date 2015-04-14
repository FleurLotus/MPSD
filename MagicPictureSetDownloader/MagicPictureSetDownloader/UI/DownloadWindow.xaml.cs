namespace MagicPictureSetDownloader.UI
{
    using System;
    using System.Windows;

    using Common.Libray.Notify;
    using Common.WPF;
    using Common.WPF.UI;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.ViewModel.Download;

    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow
    {
        public DownloadWindow()
        {
            DataContext = new DownloadViewModel(new DispatcherInvoker(Application.Current.Dispatcher), false);
            InitializeComponent();
        }
        public void CredentialRequiered(object sender, EventArgs<CredentialRequieredArgs> args)
        {
            CredentialInputViewModel vm = new CredentialInputViewModel();
            CommonDialog f = new CommonDialog(vm) { Owner = this, WindowStyle = WindowStyle.ToolWindow }; 
            f.ShowDialog();
            if (vm.Result.HasValue && vm.Result.Value)
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
            if (vm.Result.HasValue && vm.Result.Value)
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
            IDisposable disposable = DataContext as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            base.OnClosed(e);
        }
    }
}
