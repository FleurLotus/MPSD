namespace MagicPictureSetDownloader
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using Common.WPF;

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
            base.OnStartup(e);

            MainWindow mainWindow = new MainWindow();
            _started = true;
            mainWindow.Show();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ex.UserDisplay();
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Exception.UserDisplay();
            e.Handled = _started;
        }
    }
}