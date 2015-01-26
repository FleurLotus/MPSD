namespace MagicPictureSetDownloader.UI
{
    using MagicPictureSetDownloader.ViewModel.Input;

    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow
    {
        public SearchWindow(SearchViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
