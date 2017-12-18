namespace Common.WPF.UI
{
    using System;

    using Common.ViewModel.SplashScreen;

    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow
    {
        public SplashScreenWindow(SplashScreenViewModel vm)
        {
            if (vm == null)
            {
                throw new ArgumentNullException(nameof(vm));
            }
            Topmost = true;
            DataContext = vm;
            InitializeComponent();
            Topmost = false;
        }
    }
}
