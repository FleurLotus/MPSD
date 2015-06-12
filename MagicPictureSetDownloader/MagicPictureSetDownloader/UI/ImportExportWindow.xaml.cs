namespace MagicPictureSetDownloader.UI
{
    using Common.Libray.Notify;
    using Common.ViewModel.Input;

    using MagicPictureSetDownloader.ViewModel.IO;

    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for ImportExportWindow.xaml
    /// </summary>
    public partial class ImportExportWindow
    {
        public ImportExportWindow(ImportExportViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
        public void OpenFileDialog(object sender, EventArgs<InputViewModel> args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                args.Data.Text = openFileDialog.FileName;
            }
        }
    }
}
