namespace MagicPictureSetDownloader.UI
{
    using System;

    using Common.Libray;
    using Common.WPF;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.ViewModel.Download;

    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow
    {
        public DownloadWindow()
        {
            DataContext = new DownloadViewModel(new DispatcherInvoker(), false);
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
        public void NewEditionCreated(object sender, EventArgs<string> args)
        {
            EditionInfosViewModel vm = new EditionInfosViewModel(args.Data);
            EditionInfosWindow f = new EditionInfosWindow(vm) { Owner = this };
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
