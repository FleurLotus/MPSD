namespace Common.Web
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Http;
    using Common.Library.Notify;
    using Common.Library.Threading;

    public class WebAccess
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private HttpClient _httpClient;
        private ICredentials _credentials;
        private readonly IDictionary<string, string> _htmlCache;
        private readonly object _lock = new object();

        public WebAccess()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
                baseurl = baseurl.Substring(0, baseurl.LastIndexOf("/", StringComparison.InvariantCulture) + 1);
            }

            if (relativeurl.StartsWith("/"))
            {
                relativeurl = relativeurl.Substring(1);
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

                url = url.Substring(0, start + 1) + url.Substring(index + pathBack.Length);
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

            return url.Substring(0, index + 1);
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
                if (_httpClient == null)
                {
                    _httpClient = NewHttpClient();
                }

                return _httpClient;
            }
        }

        private HttpClient NewHttpClient()
        {
            if (_credentials == null)
            {
                return new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            }
            return new HttpClient(new HttpClientHandler { Credentials = _credentials });
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

        public byte[] GetFile(string url)
        {
            return AsyncHelper.RunSync(() => GetFileAsync(url));
        }

        public async Task<byte[]> GetFileAsync(string url)
        {
            return await GetDataWithProxyFallBack(() => GetHttpClient().GetByteArrayAsync(url));
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
