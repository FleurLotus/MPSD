namespace Common.ViewModel.SplashScreen
{
    using System;

    public static class SplashScreenFactory
    {
        private static readonly Lazy<SplashScreenViewModel> _lazy = new Lazy<SplashScreenViewModel>(() => new SplashScreenViewModel());

        public static SplashScreenViewModel CreateOrGetSplashScreen()
        {
            return _lazy.Value;
        }
    }
}
