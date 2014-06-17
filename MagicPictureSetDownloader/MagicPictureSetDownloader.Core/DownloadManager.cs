using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using CommonLibray;

namespace MagicPictureSetDownloader.Core
{
    public class DownloadManager
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;
        private ICredentials _credentials;
        private readonly InfoParser _infoParser;
        private readonly IDictionary<string, string> _htmlCache;

        public DownloadManager()
        {
            _htmlCache = new Dictionary<string, string>();
            _infoParser = new InfoParser();
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
                    _credentials = new NetworkCredential {UserName = args.Login, Password = args.Password};
                    return true;
                }
            }

            return false;
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

        public IEnumerable<SetInfo> GetSetList(string url)
        {
            string htmltext = GetHtml(url);
            return _infoParser.ParseSetsList(htmltext);
        }
        public string GetName(string url)
        {
            string htmltext = GetHtml(url);
            return _infoParser.ParseName(htmltext);
        }
        public PictureInfo[] GetPicturesList(string url)
        {
            string htmltext = GetHtml(url);
            return _infoParser.ParsePicturesList(htmltext);
        }
        public void GetPicture(string pictureurl, string outputPath, string name)
        {
            string namefromurl = Path.GetFileName(pictureurl);

            if (string.IsNullOrWhiteSpace(name))
                name = namefromurl;
            else
                name += Path.GetExtension(namefromurl);

            name = string.Join(string.Empty,name.Split(Path.GetInvalidFileNameChars()));
            GetFile(pictureurl, Path.Combine(outputPath, name));
        }

        private WebClient GetWebClient()
        {
            WebClient webClient = new WebClient();
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
