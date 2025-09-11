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
            ArgumentNullException.ThrowIfNull(vm);
            Topmost = true;
            DataContext = vm;
            InitializeComponent();
            Topmost = false;
        }
    }
}