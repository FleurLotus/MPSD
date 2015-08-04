namespace Common.WPF
{
    using System;

    using Common.Library;
    using Common.ViewModel.SplashScreen;
    using Common.WPF.UI;

    internal sealed class SplashScreenWrapper: ISplashScreen
    {
        private bool _disposed;
        private readonly SplashScreenWindow _window;
        private readonly SplashScreenViewModel _viewModel;

        public SplashScreenWrapper(Uri imageUri, bool showProgress, string text)
        {
            _viewModel = new SplashScreenViewModel { CurrentValue = 0, SourceUri = imageUri, ShowProgess = showProgress, Info = text };
            _window = new SplashScreenWindow(_viewModel);
            _window.Show();
        }
        public void Progress(int perCent)
        {
            if (perCent < 0)
                perCent = 0;

            if (perCent > 100)
                perCent = 100;

            _viewModel.CurrentValue = perCent;
        }
        public void ChangeDisplayText(string text)
        {
            _viewModel.Info = text;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_window != null)
                {
                    _window.Close();
                }
            }
            _disposed = true;
        }
    }
}
