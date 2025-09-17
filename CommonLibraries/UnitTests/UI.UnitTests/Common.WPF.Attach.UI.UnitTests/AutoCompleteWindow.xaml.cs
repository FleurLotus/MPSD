namespace Common.WPF.Attach.UI.UnitTests
{
    public partial class AutoCompleteWindow
    {
        public AutoCompleteWindow()
        {
            DataContext = new AutoCompleteWindowViewModel();
            InitializeComponent();
        }
    }
}