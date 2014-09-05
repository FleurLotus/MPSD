namespace MagicPictureSetDownloader
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using Common.WPF;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            DispatcherUnhandledException += ApplicationDispatcherUnhandledException;
            base.OnStartup(e);
        }
        

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ex.UserDisplay();
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Exception.UserDisplay();
            e.Handled = true;
        }
    }
}