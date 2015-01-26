namespace MagicPictureSetDownloader.UI
{
    using System;
    using System.Windows;

    using Common.Libray.Notify;
    using Common.ViewModel;
    using Common.WPF;
    using Common.WPF.UI;

    using MagicPictureSetDownloader.ViewModel.Input;
    using MagicPictureSetDownloader.ViewModel.Main;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(new DispatcherInvoker(Application.Current.Dispatcher));
            InitializeComponent();
        }

        public void UpdateImageDatabaseRequested(object sender, EventArgs args)
        {
            new DownloadImageWindow { Owner = this }.ShowDialog();
        }
        public void UpdateDatabaseRequested(object sender, EventArgs args)
        {
            new DownloadWindow { Owner = this }.ShowDialog();
        }
        public void VersionRequested(object sender, EventArgs args)
        {
            new VersionWindow { Owner = this }.ShowDialog();
        }
        public void InputRequested(object sender, EventArgs<InputViewModel> args)
        {
            new InputDialog(args.Data) { Owner = this }.ShowDialog();
        }
        public void ImportExportWanted(object sender, EventArgs args)
        {
            new ImportExportWindow { Owner = this }.ShowDialog();
        }
        public void AddCardWanted(object sender, EventArgs<CardInputViewModel> args)
        {
            new CardInputWindow(args.Data) { Owner = this }.ShowDialog();
        }
        public void UpdateCardWanted(object sender, EventArgs<CardUpdateViewModel> args)
        {
            new CardUpdateWindow(args.Data) { Owner = this }.ShowDialog();
        }
        public void Search(object sender, EventArgs<SearchViewModel> args)
        {
            new SearchWindow(args.Data) { Owner = this }.ShowDialog();
        }

    }
}
