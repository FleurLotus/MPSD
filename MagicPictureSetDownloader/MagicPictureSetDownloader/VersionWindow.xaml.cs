namespace MagicPictureSetDownloader
{
    using Common.ViewModel;

    /// <summary>
    /// Interaction logic for VersionWindow.xaml
    /// </summary>
    public partial class VersionWindow
    {
        public VersionWindow()
        {
            DataContext = new VersionViewModel();
            InitializeComponent();
        }
    }
}
