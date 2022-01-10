namespace MagicPictureSetDownloader
{
    using System;
    using System.Configuration;
#if DEBUG
    using System.Diagnostics;
#endif
    using System.Windows;
    using System.Windows.Threading;

#if !DEBUG
    using Common.ViewModel.SplashScreen;
    using Common.WPF.UI;
#endif
    using Common.WPF;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.UI;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private bool _started;

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            DispatcherUnhandledException += ApplicationDispatcherUnhandledException;
            string softwareRenderMode = ConfigurationManager.AppSettings["SoftwareRenderMode"];
            if (string.Compare(softwareRenderMode, "TRUE", true) == 0)
            {
                Lib.SoftwareRenderMode = true;
            }

            base.OnStartup(e);

            MainWindow mainWindow;
#if !DEBUG
            SplashScreenViewModel splashScreen = SplashScreenFactory.CreateOrGetSplashScreen();
            splashScreen.SourceUri = new Uri("pack://application:,,,/Resources/Splash.jpg");
            splashScreen.ShowProgress = false;
            splashScreen.Info = "Loading ...";
            SplashScreenWindow splashScreenWindow = new SplashScreenWindow(splashScreen);

            try
            {
                splashScreenWindow.Show();
#endif
                MagicDatabaseManager.Initialise(MultiPartCardManager.Instance);
                mainWindow = new MainWindow();
#if !DEBUG
            }
            finally
            {
                splashScreenWindow.Close();
            }
#endif
            _started = true;
            mainWindow.Show();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Dispatcher.Invoke((Action)(ex.UserDisplay));
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Exception.UserDisplay();
            e.Handled = _started;
        }
    }
}