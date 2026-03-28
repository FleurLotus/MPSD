namespace MagicPictureSetDownloader.UI
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Common.Notify;
    using Common.ViewModel.Web;
    using Common.Web;
    using Common.WPF;
    using Common.WPF.UI;

    using MagicPictureSetDownloader.ViewModel.Download;

    /// <summary>
    /// Interaction logic for DownloadImageWindow.xaml
    /// </summary>
    public partial class AutoDownloadWindow
    {
        private readonly DownloadViewModelBase _vm;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public AutoDownloadWindow(DownloadViewModelBase vm)
        {
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
            DataContext = vm;
            InitializeComponent();

            // Start asynchronously once the Window is loaded (runs on UI thread)
            Loaded += AutoDownloadWindow_Loaded;
        }

        private async void AutoDownloadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= AutoDownloadWindow_Loaded;

            try
            {
                await _vm.Start(new DispatcherInvoker(Application.Current.Dispatcher), _cts.Token).ConfigureAwait(true);
            }
            catch (Exception)
            {
                // expected when cancelled
            }
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
            // cancel running operation started by the window
            try
            {
                _cts.Cancel();
            }
            catch
            {
                // ignore
            }
            _cts.Dispose();

            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnClosed(e);
        }
    }
}