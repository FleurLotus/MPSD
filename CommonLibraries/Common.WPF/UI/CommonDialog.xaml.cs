namespace Common.WPF.UI
{
    using Common.Library.Notify;
    using Common.ViewModel.Dialog;
    using Common.ViewModel.Input;

    /// <summary>
    /// Interaction logic for CommonDialog.xaml
    /// </summary>
    public partial class CommonDialog
    {
        public CommonDialog(DialogViewModelBase vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
        public void DialogWanted(object sender, EventArgs<DialogViewModelBase> args)
        {
            new CommonDialog(args.Data) { Owner = this }.ShowDialog();
        }
        public void InputRequested(object sender, EventArgs<InputViewModel> args)
        {
            new InputDialog(args.Data) { Owner = this }.ShowDialog();
        }
    }
}
