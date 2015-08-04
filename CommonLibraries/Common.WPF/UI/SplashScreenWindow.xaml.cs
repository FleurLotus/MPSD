namespace Common.WPF.UI
{
    using System;

    using Common.ViewModel.SplashScreen;

    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    internal partial class SplashScreenWindow
    {
        public SplashScreenWindow(SplashScreenViewModel vm)
        {
            if (vm == null)
                throw new ArgumentNullException("vm");

            DataContext = vm;
            InitializeComponent();
        }
    }
}
