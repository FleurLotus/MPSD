namespace MagicPictureSetDownloader.UI
{
    using MagicPictureSetDownloader.ViewModel.Deck;

    /// <summary>
    /// Interaction logic for PreconstructedDecksWindow.xaml
    /// </summary>
    public partial class PreconstructedDecksWindow
    {
        public PreconstructedDecksWindow(PreconstructedDecksViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
