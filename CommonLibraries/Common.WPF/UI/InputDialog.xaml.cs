namespace Common.WPF.UI
{
    using Common.ViewModel.Input;

    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog 
    {
        public InputDialog(InputViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
