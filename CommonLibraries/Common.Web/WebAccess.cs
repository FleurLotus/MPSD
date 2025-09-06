namespace Common.Web
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Http;
    using System.IO;

    using Common.Notify;
    using Common.Threading;

    public class WebAccess
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private HttpClient _httpClient;
        private ICredentials _credentials;
        private readonly Dictionary<string, string> _htmlCache;
        private readonly object _lock = new object();
        private readonly TimeSpan? _timeout;
        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;

        public WebAccess(TimeSpan? timeOut = null): this(new HttpMessageHandlerFactory(), timeOut)
        {
        }
        internal WebAccess(IHttpMessageHandlerFactory httpMessageHandlerFactory, TimeSpan? timeOut = null)
        {
            _httpMessageHandlerFactory = httpMessageHandlerFactory;
            _timeout = timeOut;
            _httpClient = GetHttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");
            _htmlCache = new Dictionary<string, string>();
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
                    _credentials = new NetworkCredential { UserName = args.Login, Password = args.Password };
                    lock (_lock)
                    {
                        _httpClient = NewHttpClient();
                    }

                    return true;
                }
            }

            return false;
        }

        private HttpClient GetHttpClient()
        {
            lock(_lock)
            {
                _httpClient ??= NewHttpClient();

                return _httpClient;
            }
        }

        private HttpClient NewHttpClient()
        {
            HttpClient client = new HttpClient(_httpMessageHandlerFactory.Create(_credentials));

            if (_timeout.HasValue)
            {
                client.Timeout = _timeout.Value;
            }

            return client;
        }

        public string GetHtml(string url, bool forceRefresh = false)
        {
            return AsyncHelper.RunSync(() => GetHtmlAsync(url, forceRefresh));
        }

        public async Task<string> GetHtmlAsync(string url, bool forceRefresh = false)
        {
            if (forceRefresh || !_htmlCache.TryGetValue(url, out string html))
            {
                html = await GetDataWithProxyFallBack(() => GetHttpClient().GetStringAsync(url));
                _htmlCache[url] = html;
            }
            return html;
        }

        public void DownloadFile(string url, string outfilepath)
        {
            AsyncHelper.RunSync(() => DownloadFileAsync(url, outfilepath));
        }

        public async Task DownloadFileAsync(string url, string outfilepath)
        {
            await GetDataWithProxyFallBack(() => DownloadFileInternalAsync(url, outfilepath));
        }

        private async Task DownloadFileInternalAsync(string url, string outfilepath)
        {
            HttpResponseMessage response = await GetHttpClient().GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            using (FileStream fs = new FileStream(outfilepath, FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }
        }

        public byte[] GetFile(string url)
        {
            return AsyncHelper.RunSync(() => GetFileAsync(url));
        }

        public async Task<byte[]> GetFileAsync(string url)
        {
            return await GetDataWithProxyFallBack(() => GetHttpClient().GetByteArrayAsync(url));
        }

        private async Task GetDataWithProxyFallBack(Func<Task> getdata)
        {
            do
            {
                try
                {
                    await getdata();
                    return;
                }
                catch (WebException wex)
                {
                    if (!wex.Message.Contains("407") || !OnCredentialRequiered())
                    {
                        throw;
                    }
                }

            } while (true);
        }

        private async Task<T> GetDataWithProxyFallBack<T>(Func<Task<T>> getdata)
        {
            do
            {
                try
                {
                    return await getdata();
                }
                catch (WebException wex)
                {
                    if (!wex.Message.Contains("407") || !OnCredentialRequiered())
                    {
                        throw;
                    }
                }

            } while (true);
        }
    }
}
