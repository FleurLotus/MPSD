using System;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Threading;
using CommonViewModel;

namespace MagicPictureSetDownloader.ViewModel
{
    internal class DownloadManager
    {
        public event EventHandler<EventArgs<string>> RunCompleted;
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;
        public event EventHandler<EventArgs<Exception>> RunError;
        private readonly WebClient _webclient;

        public DownloadManager()
        {
            _webclient = new WebClient();
        }

        public DownloadManager(string userName, string password): this()
        {
            _webclient.Proxy.Credentials = new NetworkCredential {UserName = userName, Password =  password};
        }
        private void OnRunError(Exception exception)
        {
            var e = RunError;
            if (e != null)
                e(this, new EventArgs<Exception>(exception));
        }
        private bool OnCredentialRequiered()
        {
            var e = CredentialRequiered;
            if (e != null)
            {
                CredentialRequieredArgs args = new CredentialRequieredArgs();

                e(this, new EventArgs<CredentialRequieredArgs>(args));

                if (!string.IsNullOrEmpty(args.Login))
                {
                    _webclient.Proxy.Credentials = new NetworkCredential {UserName = args.Login, Password = args.Password};
                    return true;
                }
            }

            return false;
        }
        private void OnRunCompleted(string message)
        {
            var e = RunCompleted;
            if (e != null)
                e(this, new EventArgs<string>(message));
        }

        internal void RunAsync(DownloadManagerRunArgs args)
        {
            ThreadPool.QueueUserWorkItem(Run, args);
        }
        private void Run(object args)
        {
            try
            {
                string baseseturl = ((DownloadManagerRunArgs)args).BaseSetUrl;
                string htmltext = GetHtml(baseseturl);
            }
            catch (Exception ex)
            {
                OnRunError(ex);
            }
        }

        private string GetHtml(string url)
        {
            bool retry;
            do
            {
                try
                {
                    return _webclient.DownloadString(url);
                }
                catch (WebException wex)
                {
                    if (wex.Message.Contains("407"))
                    {
                        retry = OnCredentialRequiered();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            } while (retry);

            return null;
        }
    }
}
