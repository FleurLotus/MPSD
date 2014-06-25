using Common.WPF;
using CommonLibray;
using MagicPictureSetDownloader.Core;
using MagicPictureSetDownloader.ViewModel;

namespace MagicPictureSetDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(new DispatcherInvoker());
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
    }
}
