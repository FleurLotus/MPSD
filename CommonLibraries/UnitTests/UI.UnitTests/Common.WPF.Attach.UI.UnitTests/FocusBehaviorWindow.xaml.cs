namespace Common.WPF.Attach.UI.UnitTests
{
    public partial class FocusBehaviorWindow
    {
        public FocusBehaviorWindow()
        {
            DataContext = new FocusBehaviorWindowViewModel();
            InitializeComponent();
        }
    }
}