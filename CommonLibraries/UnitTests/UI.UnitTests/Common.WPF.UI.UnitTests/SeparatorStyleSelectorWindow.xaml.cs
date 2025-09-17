namespace Common.WPF.UI.UnitTests
{
    public partial class SeparatorStyleSelectorWindow
    {
        public SeparatorStyleSelectorWindow()
        {
            DataContext = new SeparatorStyleSelectorWindowViewModel();
            InitializeComponent();
        }
    }
}