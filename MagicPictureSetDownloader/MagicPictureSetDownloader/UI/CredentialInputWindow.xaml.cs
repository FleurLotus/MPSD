namespace MagicPictureSetDownloader.UI
{
    using MagicPictureSetDownloader.ViewModel.Download;

    /// <summary>
    /// Interaction logic for CredentialInput.xaml
    /// </summary>
    public partial class CredentialInputWindow
    {
        public CredentialInputWindow(CredentialInputViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
