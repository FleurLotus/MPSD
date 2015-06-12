namespace Common.WPF.UI
{
    using Common.ViewModel.Version;

    /// <summary>
    /// Interaction logic for VersionWindow.xaml
    /// </summary>
    public partial class VersionWindow
    {
        public VersionWindow()
        {
            DataContext = new VersionViewModel();
            InitializeComponent();
        }
    }
}
