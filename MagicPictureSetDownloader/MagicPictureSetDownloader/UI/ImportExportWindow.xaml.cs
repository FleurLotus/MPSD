
namespace MagicPictureSetDownloader.UI
{
    using MagicPictureSetDownloader.ViewModel.IO;

    /// <summary>
    /// Interaction logic for ImportExportWindow.xaml
    /// </summary>
    public partial class ImportExportWindow
    {
        public ImportExportWindow()
        {
            DataContext = new ImportExportViewModel();
            InitializeComponent();
        }
    }
}
