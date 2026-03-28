namespace Common.Web
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Common.Notify;
    using Common.Threading;

    public class WebAccess
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private HttpClient _httpClient;
        private ICredentials _credentials;
        private readonly ConcurrentDictionary<string, string> _htmlCache;
#if NET9_0_OR_GREATER
        private readonly System.Threading.Lock _lock = new System.Threading.Lock();
#else
        private readonly object _lock = new object();
#endif
        private readonly TimeSpan? _timeout;
        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;

        public WebAccess(TimeSpan? timeOut = null) : this(new HttpMessageHandlerFactory(), timeOut)
        {
        }
        internal WebAccess(IHttpMessageHandlerFactory httpMessageHandlerFactory, TimeSpan? timeOut = null)
        {
            _httpMessageHandlerFactory = httpMessageHandlerFactory;
            _timeout = timeOut;
            _httpClient = GetHttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");
            _htmlCache = new ConcurrentDictionary<string, string>();
        }

        private bool OnCredentialRequiered()
        {
            EventHandler<EventArgs<CredentialRequieredArgs>> e = CredentialRequiered;
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
            lock (_lock)
            {
                _httpClient ??= NewHttpClient();

                return _httpClient;
            }
        }

        private HttpClient NewHttpClient()
        {
            HttpClient client = new HttpClient(_httpMessageHandlerFactory.Create(_credentials));

            // Required by Scryfall: meaningful User-Agent and Accept header
            // Use an identifiable app string and a contact URL per Scryfall docs
            // Example: "MPSD/1.0 (+https://github.com/FleurLotus/MPSD)"
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MPSD/1.0 (+https://github.com/FleurLotus/MPSD)");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            if (_timeout.HasValue)
            {
                client.Timeout = _timeout.Value;
            }

            return client;
        }

        public async Task<string> GetHtmlAsync(string url, bool forceRefresh = false, CancellationToken ct = default)
        {
            if (forceRefresh || !_htmlCache.TryGetValue(url, out string html))
            {
                html = await GetDataWithProxyFallBack(async () =>
                {
                    // Use GetAsync so we can read the response body even when status is non-success
                    using (HttpResponseMessage response = await GetHttpClient().GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
                    {
                        string content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            // Include status and body to help diagnose API error
                            string message = $"Request to '{url}' failed {(int) response.StatusCode} {response.ReasonPhrase}. Response body: {content}";

                            throw new WebException(message);
                        }

                        return content;
                    }
                }).ConfigureAwait(false);

                _htmlCache[url] = html;
            }
            return html;
        }

        public async Task DownloadFileAsync(string url, string outfilepath, CancellationToken ct = default)
        {
            await GetDataWithProxyFallBack(() => DownloadFileInternalAsync(url, outfilepath, ct)).ConfigureAwait(false);
        }

        private async Task DownloadFileInternalAsync(string url, string outfilepath, CancellationToken ct)
        {
            using (HttpResponseMessage response = await GetHttpClient().GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException($"{(int) response.StatusCode} {response.ReasonPhrase}");
                }

                using (FileStream fs = new FileStream(outfilepath, FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(fs, ct).ConfigureAwait(false);
                }
            }
        }

        public async Task<byte[]> GetFileAsync(string url, CancellationToken ct = default)
        {
            return await GetDataWithProxyFallBack(() => GetHttpClient().GetByteArrayAsync(url, ct)).ConfigureAwait(false);
        }

        private async Task GetDataWithProxyFallBack(Func<Task> getdata)
        {
            do
            {
                try
                {
                    await getdata().ConfigureAwait(false);
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
                    return await getdata().ConfigureAwait(false);
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