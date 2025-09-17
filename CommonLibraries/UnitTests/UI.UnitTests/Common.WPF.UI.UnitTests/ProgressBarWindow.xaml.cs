namespace Common.WPF.UI.UnitTests
{
    public partial class ProgressBarWindow
    {
        public ProgressBarWindow()
        {
            DataContext = new ProgressBarWindowViewModel();
            InitializeComponent();
        }
    }
}