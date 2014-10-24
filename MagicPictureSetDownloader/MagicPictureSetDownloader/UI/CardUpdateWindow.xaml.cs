namespace MagicPictureSetDownloader.UI
{
    using MagicPictureSetDownloader.ViewModel.Input;

    /// <summary>
    /// Interaction logic for CardUpdateWindow.xaml
    /// </summary>
    public partial class CardUpdateWindow
    {
        public CardUpdateWindow(CardUpdateViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
