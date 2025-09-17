namespace Common.WPF.Behavior.UI.UnitTests
{
    public partial class BindableSelectedItemBehaviorWindow
    {
        public BindableSelectedItemBehaviorWindow()
        {
            DataContext = new BindableSelectedItemBehaviorWindowViewModel();
            InitializeComponent();
        }
    }
}