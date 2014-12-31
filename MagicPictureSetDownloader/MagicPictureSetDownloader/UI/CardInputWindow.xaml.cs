namespace MagicPictureSetDownloader.UI
{
    using Common.Libray;
    using Common.Libray.Notify;
    using Common.ViewModel;
    using Common.WPF.UI;

    using MagicPictureSetDownloader.ViewModel.Input;

    /// <summary>
    /// Interaction logic for CardInputWindow.xaml
    /// </summary>
    public partial class CardInputWindow
    {
        public CardInputWindow(CardInputViewModel vm)
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
