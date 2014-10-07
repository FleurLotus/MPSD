namespace MagicPictureSetDownloader.UI
{
    using System;

    using Common.Libray;
    using Common.ViewModel;
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
            DataContext = new MainViewModel();
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
    }
}
