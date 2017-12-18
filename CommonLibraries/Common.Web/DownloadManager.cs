namespace Common.Web
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Common.Library.Notify;

    public class WebAccess
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;

        private ICredentials _credentials;
        private readonly IDictionary<string, string> _htmlCache = new Dictionary<string, string>();

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
                    return true;
                }
            }

            return false;
        }

        private HttpClient GetHttpClient()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (_credentials == null)
            {
                httpClientHandler.UseDefaultCredentials = true;
            }
            else
            {
                httpClientHandler.Credentials = _credentials;
            }

            return new HttpClient(httpClientHandler);

        }
        public string GetHtml(string url, bool forceRefresh = false)
        {
            string html;
            if (forceRefresh || !_htmlCache.TryGetValue(url, out html))
            {
                html = GetDataWithProxyFallBack(() =>
                {
                    using (HttpClient httpClient = GetHttpClient())
                    {
                        return httpClient.GetStringAsync(url).Result;
                    }
                });
                _htmlCache[url] = html;
            }
            return html;
        }
        public byte[] GetFile(string url)
        {
            return GetDataWithProxyFallBack(
                () =>
                    {
                        using (HttpClient httpClient = GetHttpClient())
                        {
                            return httpClient.GetByteArrayAsync(url).Result;
                        }
                    }
               );
        }
        private T GetDataWithProxyFallBack<T>(Func<T> getdata)
        {
            do
            {
                try
                {
                    return getdata();
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
