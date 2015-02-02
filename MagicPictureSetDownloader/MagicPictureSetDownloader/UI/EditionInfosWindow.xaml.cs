namespace MagicPictureSetDownloader.UI
{
    using MagicPictureSetDownloader.ViewModel.Download;

    /// <summary>
    /// Interaction logic for EditionInfosWindow.xaml
    /// </summary>
    public partial class EditionInfosWindow
    {
        public EditionInfosWindow(NewEditionInfoViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
