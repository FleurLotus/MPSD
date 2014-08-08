namespace MagicPictureSetDownloader
{
    using Common.WPF;
    using Common.Libray;
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
            CredentialInput f = new CredentialInput(vm) {Owner = this};
            f.ShowDialog();
            if (vm.Result.HasValue && vm.Result.Value)
            {
                args.Data.Login = vm.Login;
                args.Data.Password = vm.Password;
            }
        }

        //TODO: to be designed 
    }
}
