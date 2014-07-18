using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Common.Drawing;
using Common.Libray;
using MagicPictureSetDownloader.Db;

namespace MagicPictureSetDownloader.Core
{
    public class DownloadManager
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;
        private ICredentials _credentials;
        private readonly IDictionary<string, string> _htmlCache;

        public DownloadManager()
        {
            _htmlCache = new Dictionary<string, string>();
        }
        
        public static string ToAbsoluteUrl(string baseurl, string relativeurl, bool useOnlyDomain = false)
        {
            if (string.IsNullOrWhiteSpace(relativeurl))
                return baseurl;

            if (string.IsNullOrWhiteSpace(baseurl))
                return relativeurl;

            if (relativeurl.Contains("//"))
                return relativeurl;

            if (useOnlyDomain)
                baseurl = ExtractBaseUrl(baseurl);

            if (!baseurl.EndsWith("/"))
                baseurl = baseurl.Substring(0, baseurl.LastIndexOf("/", StringComparison.Ordinal) + 1);

            if (relativeurl.StartsWith("/"))
                relativeurl = relativeurl.Substring(1);

            return baseurl + relativeurl;
        }
        private static string ExtractBaseUrl(string url)
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
            foreach (SetInfo setInfo in Parser.ParseSetsList(htmltext))
            {
                IEdition edition = MagicDatabaseManager.Instance.GetEdition(setInfo.Name);
                yield return new SetInfoWithBlock(setInfo, edition);
            }
        }
        public string[] GetCardUrls(string url)
        {
            string htmltext = GetHtml(url);
            return Parser.ParseCardUrls(htmltext).ToArray();
        }
        public void GetCardInfo(string url, int editionId)
        {
            string htmltext = GetHtml(url);

            CardWithExtraInfo cardWithExtraInfo = Parser.ParseCardInfo(htmltext);

            string pictureUrl = ToAbsoluteUrl(url, cardWithExtraInfo.PictureUrl);
            InsertPictureInDb(pictureUrl);
            InsertCardInDb(cardWithExtraInfo);
            InsertCardSetInDb(editionId, cardWithExtraInfo);
        }

        private void InsertCardSetInDb(int idEdition, CardWithExtraInfo cardWithExtraInfo)
        {
            int idGatherer = Parser.ExtractIdGatherer(cardWithExtraInfo.PictureUrl);

            MagicDatabaseManager.Instance.InsertNewCardEdition(idGatherer, idEdition, cardWithExtraInfo.Name, cardWithExtraInfo.Rarity);
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
        private void InsertPictureInDb(string pictureUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(pictureUrl);

            IPicture picture = MagicDatabaseManager.Instance.GetPicture(idGatherer);
            if (picture == null)
            {
                //No id found try insert
                byte[] pictureData = GetFile(pictureUrl);

                MagicDatabaseManager.Instance.InsertNewPicture(idGatherer, pictureData);
            }
            else // if (picture.Image == null)
            {
               //ALERT: manage update 
                object o = Converter.BytesToImage(picture.Image);
            }

        }
        private void InsertCardInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            MagicDatabaseManager.Instance.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Text, cardWithExtraInfo.Power, cardWithExtraInfo.Toughness,
                                                        cardWithExtraInfo.CastingCost, cardWithExtraInfo.Loyalty, cardWithExtraInfo.Type);
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
        private byte[] GetFile(string url)
        {
            return GetDataWithProxyFallBack(
                () =>
                    {
                        using (WebClient webClient = GetWebClient())
                            return webClient.DownloadData(url);
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
