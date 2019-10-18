namespace MagicPictureSetDownloader.UI
{
    using Common.Library.Notify;
    using Common.ViewModel.Input;
    using Common.WPF.UI;
    using MagicPictureSetDownloader.ViewModel.Input;

    /// <summary>
    /// Interaction logic for CollectionInputGraphicWindow.xaml
    /// </summary>
    public partial class CollectionInputGraphicWindow
    {
        public CollectionInputGraphicWindow(CollectionInputGraphicViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
        public void InputRequested(object sender, EventArgs<InputViewModel> args)
        {
            new InputDialog(args.Data) { Owner = this }.ShowDialog();
        }
    }
}
