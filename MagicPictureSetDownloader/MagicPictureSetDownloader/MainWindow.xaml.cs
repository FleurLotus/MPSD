namespace MagicPictureSetDownloader
{
    using System;
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

        public void UpdateDatabaseRequested(object sender, EventArgs args)
        {
            new DownloadWindow().ShowDialog();
        }
        public void VersionRequested(object sender, EventArgs args)
        {
            new VersionWindow().ShowDialog();
        }
    }
}
