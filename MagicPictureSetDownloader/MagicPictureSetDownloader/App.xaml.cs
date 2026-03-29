namespace MagicPictureSetDownloader
{
    using System;
    using System.Configuration;
    using System.Windows;
    using System.Windows.Threading;

#if !DEBUG
    using Common.ViewModel.SplashScreen;
    using Common.WPF.UI;
#endif
    using Common.Log;
    using Common.WPF;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.UI;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private bool _started;
        private ILogger _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            LogManager.Factory = LoggerFactory.Create(builder => builder.AddProvider(new FileLoggerProvider("logs.txt")));

            _logger = LogManager.Factory.CreateLogger<App>();
            _logger.LogInformation("*** Start application ***");

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
            MagicDatabaseManager.Initialise();
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
            _logger.LogError(ex, "Unhandled exception");
            Dispatcher.Invoke((Action) (ex.UserDisplay));
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, "Unhandled exception");
            e.Exception.UserDisplay();
            e.Handled = _started;
        }
    }
}