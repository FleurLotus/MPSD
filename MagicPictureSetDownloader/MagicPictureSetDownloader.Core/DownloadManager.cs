namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Common.Libray;
    using MagicPictureSetDownloader.Interface;

    public class DownloadManager
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;
        private ICredentials _credentials;
        private readonly IDictionary<string, string> _htmlCache;
        private readonly MagicDatabaseManager _magicDatabaseManager;

        public DownloadManager()
        {
            _magicDatabaseManager = new MagicDatabaseManager();
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
                baseurl = baseurl.Substring(0, baseurl.LastIndexOf("/", StringComparison.InvariantCulture) + 1);

            if (relativeurl.StartsWith("/"))
                relativeurl = relativeurl.Substring(1);

            return RemovePathBack(baseurl + relativeurl);
        }

        private static string RemovePathBack(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            const string pathBack = "/../";
            int index;
            while ((index = url.IndexOf(pathBack, StringComparison.InvariantCultureIgnoreCase)) >= 0)
            {
                int start = url.LastIndexOf('/', index - 1);
                if (start < 0)
                    continue;

                url = url.Substring(0, start + 1) + url.Substring(index + pathBack.Length);
            }
            return url;

        }
        private static string ExtractBaseUrl(string url)
        {
            const string postfixProtocol = @"://";

            if (string.IsNullOrWhiteSpace(url))
                return url;


            int startIndex = url.IndexOf(postfixProtocol, StringComparison.InvariantCulture);
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
                IEdition edition = _magicDatabaseManager.GetEdition(setInfo.Name);
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

            foreach (CardWithExtraInfo cardWithExtraInfo in Parser.ParseCardInfo(htmltext))
            {
                string pictureUrl = ToAbsoluteUrl(url, cardWithExtraInfo.PictureUrl);

                InsertCardInDb(cardWithExtraInfo);
                InsertCardSetInDb(editionId, cardWithExtraInfo, pictureUrl);
            }
        }
        public void InsertPictureInDb(string pictureUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(pictureUrl);

            IPicture picture = _magicDatabaseManager.GetPicture(idGatherer);
            if (picture == null)
            {
                //No id found try insert
                byte[] pictureData = GetFile(pictureUrl);

                _magicDatabaseManager.InsertNewPicture(idGatherer, pictureData);
            }
            else
            {
                //ALERT: see if we update the data
            }
        }
        public string[] GetMissingPictureUrls()
        {
            return _magicDatabaseManager.GetMissingPictureUrls();
        }

        private void InsertCardSetInDb(int idEdition, CardWithExtraInfo cardWithExtraInfo, string pictureUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(cardWithExtraInfo.PictureUrl);

            _magicDatabaseManager.InsertNewCardEdition(idGatherer, idEdition, cardWithExtraInfo.Name, cardWithExtraInfo.PartName, cardWithExtraInfo.Rarity, pictureUrl);
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

        private void InsertCardInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            _magicDatabaseManager.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Text, cardWithExtraInfo.Power, cardWithExtraInfo.Toughness,
                                                        cardWithExtraInfo.CastingCost, cardWithExtraInfo.Loyalty, cardWithExtraInfo.Type, 
                                                        cardWithExtraInfo.PartName, cardWithExtraInfo.OtherPathName);
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
