namespace MagicPictureSetDownloader
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Threading;

    using Common.ViewModel.SplashScreen;
    using Common.WPF;
    using Common.WPF.UI;

    using MagicPictureSetDownloader.UI;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private bool _started;

        protected override void OnStartup(StartupEventArgs e)
        {
            //For debugging Binding issue
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;

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