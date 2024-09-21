namespace Common.Web
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Http;
    using System.IO;

    using Common.Library.Notify;
    using Common.Library.Threading;

    public class WebAccess
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private HttpClient _httpClient;
        private ICredentials _credentials;
        private readonly IDictionary<string, string> _htmlCache;
        private readonly object _lock = new object();
        private readonly TimeSpan? _timeout;

        public WebAccess(TimeSpan? timeOut = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _timeout = timeOut;
            _httpClient = GetHttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");
            _htmlCache = new Dictionary<string, string>();
        }

        public static string ToAbsoluteUrl(string baseurl, string relativeurl, bool useOnlyDomain = false)
        {
            if (string.IsNullOrWhiteSpace(relativeurl))
            {
                return baseurl;
            }

            if (string.IsNullOrWhiteSpace(baseurl))
            {
                return relativeurl;
            }

            if (relativeurl.Contains("//"))
            {
                return relativeurl;
            }

            if (useOnlyDomain)
            {
                baseurl = ExtractBaseUrl(baseurl);
            }

            if (!baseurl.EndsWith("/"))
            {
                baseurl = baseurl[..(baseurl.LastIndexOf("/", StringComparison.InvariantCulture) + 1)];
            }

            if (relativeurl.StartsWith("/"))
            {
                relativeurl = relativeurl[1..];
            }

            return RemovePathBack(baseurl + relativeurl);
        }

        private static string RemovePathBack(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }

            const string pathBack = "/../";
            int index;
            while ((index = url.IndexOf(pathBack, StringComparison.InvariantCultureIgnoreCase)) >= 0)
            {
                int start = url.LastIndexOf('/', index - 1);
                if (start < 0)
                {
                    continue;
                }

                url = url[..(start + 1)] + url[(index + pathBack.Length)..];
            }
            return url;

        }
        private static string ExtractBaseUrl(string url)
        {
            const string postfixProtocol = @"://";

            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }

            int startIndex = url.IndexOf(postfixProtocol, StringComparison.InvariantCulture);
            if (startIndex < 0)
            {
                return url;
            }

            int index = url.IndexOf('/', startIndex + postfixProtocol.Length);
            if (index < 0)
            {
                return url;
            }

            return url[..(index + 1)];
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
            HttpClient client;

            if (_credentials == null)
            {
                client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            }
            else
            {
                client = new HttpClient(new HttpClientHandler { Credentials = _credentials });
            }
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
