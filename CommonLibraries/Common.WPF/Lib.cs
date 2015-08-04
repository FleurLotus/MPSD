namespace Common.WPF
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using Common.Library;

    public static class Lib
    {
        public static bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
        public static ISplashScreen CreateSplashScreen(Uri imageUri, bool showProgress, string text = null)
        {
            return new SplashScreenWrapper(imageUri, showProgress, text);
        }
    }
}
