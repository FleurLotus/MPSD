namespace Common.WPF.UI
{
    using Common.ViewModel;

    /// <summary>
    /// Interaction logic for InputTextDialog.xaml
    /// </summary>
    public partial class InputTextDialog 
    {
        public InputTextDialog(InputTextViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
