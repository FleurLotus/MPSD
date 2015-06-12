namespace Common.WPF.UI
{
    using Common.ViewModel.Exception;

    /// <summary>
    /// Interaction logic for ExceptionDialog.xaml
    /// </summary>
    public partial class ExceptionDialog 
    {
        public ExceptionDialog(ExceptionViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
