namespace MagicPictureSetDownloader
{
    using System;

    using Common.Libray;
    using Common.ViewModel;
    using Common.WPF.UI;

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
            new DownloadImageWindow().ShowDialog();
        }
        public void UpdateDatabaseRequested(object sender, EventArgs args)
        {
            new DownloadWindow().ShowDialog();
        }
        public void VersionRequested(object sender, EventArgs args)
        {
            new VersionWindow().ShowDialog();
        }
        public void NameRequested(object sender, EventArgs<InputTextViewModel> args)
        {
            new InputTextDialog(args.Data) { Owner = this }.ShowDialog();;
        }

    }
}
