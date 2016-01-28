namespace MagicPictureSetDownloader.UI
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using Common.Library.Notify;
    using Common.ViewModel.Dialog;
    using Common.ViewModel.Input;
    using Common.WPF;
    using Common.WPF.UI;

    using MagicPictureSetDownloader.ViewModel.Download;
    using MagicPictureSetDownloader.ViewModel.IO;
    using MagicPictureSetDownloader.ViewModel.Main;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(new DispatcherInvoker(Application.Current.Dispatcher));
            InitializeComponent();
        }

        public void AutoUpdateDatabaseRequested(object sender, EventArgs<DownloadViewModelBase> args)
        {
            new AutoDownloadWindow(args.Data) { Owner = this }.ShowDialog();
        }
        public void UpdateDatabaseRequested(object sender, EventArgs<DownloadViewModelBase> args)
        {
            new DownloadWindow(args.Data) { Owner = this }.ShowDialog();
        }
        public void VersionRequested(object sender, EventArgs args)
        {
            new VersionWindow { Owner = this }.ShowDialog();
        }
        public void InputRequested(object sender, EventArgs<InputViewModel> args)
        {
            new InputDialog(args.Data) { Owner = this }.ShowDialog();
        }
        public void ImportExportRequested(object sender, EventArgs<ImportExportViewModel> args)
        {
            new ImportExportWindow(args.Data) { Owner = this }.ShowDialog();
        }
        public void DialogWanted(object sender, EventArgs<DialogViewModelBase> args)
        {
            new CommonDialog(args.Data) { Owner = this }.ShowDialog();
        }
        public void ExceptionOccured(object sender, EventArgs<Exception> args)
        {
            args.Data.UserDisplay();
        }
        public void DatabaseModificationRequested(object sender, EventArgs<INotifyPropertyChanged> args)
        {
            new DatabaseInfoModificationWindow(args.Data) { Owner = this }.ShowDialog();
        }
    }
}
