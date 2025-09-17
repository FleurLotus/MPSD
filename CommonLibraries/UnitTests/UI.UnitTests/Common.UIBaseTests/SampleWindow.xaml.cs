namespace Common.UIBaseTests
{
    public partial class SampleWindow
    {
        public SampleWindow()
        {
            DataContext = new SampleWindowViewModel();
            InitializeComponent();
        }
    }
}