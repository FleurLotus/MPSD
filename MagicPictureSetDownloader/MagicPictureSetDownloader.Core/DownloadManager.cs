using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Common.Libray;
using MagicPictureSetDownloader.Core.Db;

namespace MagicPictureSetDownloader.Core
{
    public class DownloadManager
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;
        private ICredentials _credentials;
        private readonly InfoParser _infoParser;
        private readonly MagicDatabaseManager _magicDatabaseManager;
        private readonly IDictionary<string, string> _htmlCache;

        public DownloadManager()
        {
            _magicDatabaseManager = new MagicDatabaseManager("MagicData.sdf");
            _htmlCache = new Dictionary<string, string>();
            _infoParser = new InfoParser();
        }
        
        public static string ToAbsoluteUrl(string baseurl, string relativeurl)
        {
            if (string.IsNullOrWhiteSpace(relativeurl))
                return baseurl;

            if (string.IsNullOrWhiteSpace(baseurl))
                return relativeurl;

            if (relativeurl.Contains("//"))
                return relativeurl;

            if (!baseurl.EndsWith("/"))
                baseurl = baseurl.Substring(0, baseurl.LastIndexOf("/", StringComparison.Ordinal) + 1);

            if (relativeurl.StartsWith("/"))
                relativeurl = relativeurl.Substring(1);

            return baseurl + relativeurl;
        }
        public static string ExtractBaseUrl(string url)
        {
            const string postfixProtocol = @"://";

            if (string.IsNullOrWhiteSpace(url))
                return url;

            
            int startIndex = url.IndexOf(postfixProtocol, StringComparison.Ordinal);
            if (startIndex < 0)
                return url;

            int index = url.IndexOf('/', startIndex + postfixProtocol.Length);
            if (index < 0)
                return url;

            return url.Substring(0, index + 1);
        }
        public IEnumerable<SetInfoWithBlock> GetSetList(string url)
        {
            string htmltext = GetHtml(url);
            foreach (SetInfo setInfo in _infoParser.ParseSetsList(htmltext))
            {
                Edition edition = _magicDatabaseManager.GetEdition(setInfo.Name);
                yield return new SetInfoWithBlock(setInfo, edition);
            }
        }
        public CardInfo[] GetCardInfos(string url)
        {
            string htmltext = GetHtml(url);
            return _infoParser.ParseCardInfos(htmltext).ToArray();

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
        private WebClient GetWebClient()
        {
            WebClient webClient = new WebClient {Encoding = Encoding.UTF8};
            if (_credentials != null)
                webClient.Proxy.Credentials = _credentials;

            return webClient;
        }
        private string GetHtml(string url)
        {
            string html;
            if (!_htmlCache.TryGetValue(url, out html))
            {
                html = GetDataWithProxyFallBack(() =>
                {
                    using (WebClient webClient = GetWebClient())
                        return webClient.DownloadString(url);
                });
                _htmlCache.Add(url, html);
            }
            return html;
        }
        private void GetFile(string url, string outpath)
        {
            GetDataWithProxyFallBack(
                () =>
                    {
                        using (WebClient webClient = GetWebClient())
                            webClient.DownloadFile(url, outpath);
                        return true;
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
                        throw;
                }

            } while (true);
        }
    }
}
