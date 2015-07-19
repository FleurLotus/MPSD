namespace MagicPictureSetDownloader.UI
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
    public partial class DownloadImageWindow
    {
        public DownloadImageWindow()
        {
            DataContext = new DownloadImageViewModel(new DispatcherInvoker(Application.Current.Dispatcher));
            InitializeComponent();
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
            IDisposable disposable = DataContext as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            base.OnClosed(e);
        }
    }
}
