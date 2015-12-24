 namespace MagicPictureSetDownloader.UI
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for DatabaseInfoModificationWindow.xaml
    /// </summary>
    public partial class DatabaseInfoModificationWindow
    {
        //The real parameter should be DatabaseInfoModificationViewModelBase<T> but generics windowsare not allowed and don't want to create n windows
        public DatabaseInfoModificationWindow(INotifyPropertyChanged vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
