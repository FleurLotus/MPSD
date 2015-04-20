﻿namespace MagicPictureSetDownloader.UI
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using Common.Libray.Notify;
    using Common.ViewModel;
    using Common.WPF;
    using Common.WPF.UI;

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

        public void UpdateImageDatabaseRequested(object sender, EventArgs args)
        {
            new DownloadImageWindow { Owner = this }.ShowDialog();
        }
        public void UpdateDatabaseRequested(object sender, EventArgs args)
        {
            new DownloadWindow { Owner = this }.ShowDialog();
        }
        public void VersionRequested(object sender, EventArgs args)
        {
            new VersionWindow { Owner = this }.ShowDialog();
        }
        public void InputRequested(object sender, EventArgs<InputViewModel> args)
        {
            new InputDialog(args.Data) { Owner = this }.ShowDialog();
        }
        public void ImportExportRequested(object sender, EventArgs args)
        {
            new ImportExportWindow { Owner = this }.ShowDialog();
        }
        public void DialogWanted(object sender, EventArgs<DialogViewModelBase> args)
        {
            new CommonDialog(args.Data) { Owner = this }.ShowDialog();
        }
        public void DatabaseModificationRequested(object sender, EventArgs<INotifyPropertyChanged> args)
        {
            new DatabaseInfoModificationWindow(args.Data) { Owner = this }.ShowDialog();
        }
    }
}
