namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using Common.Library.Notify;

    using MagicPictureSetDownloader.Core.EditionInfos;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class DownloadManager
    {
        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered;
        public event EventHandler<EventArgs<string>> NewEditionCreated;

        private ICredentials _credentials;
        private readonly IDictionary<string, string> _htmlCache;
        private readonly IMagicDatabaseReadAndWriteReference _magicDatabase;

        public DownloadManager()
        {
            _magicDatabase = MagicDatabaseManager.ReadAndWriteReference;
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

        public IEnumerable<EditionInfoWithBlock> GetEditionList(string url)
        {
            string htmltext = GetHtml(url);
            foreach (EditionInfo editionInfo in Parser.ParseEditionsList(htmltext))
            {
                IEdition edition = _magicDatabase.GetEdition(editionInfo.Name);
                if (edition == null)
                {
                    OnNewEditionCreated(editionInfo.Name);
                    edition = _magicDatabase.GetEdition(editionInfo.Name);
                }
                yield return new EditionInfoWithBlock(editionInfo, edition);
            }
        }
        public string[] GetCardUrls(string url)
        {
            List<string> ret = new List<string>();
            
            ManageMultiPage(url, html =>
            {
                foreach (string cardurl in Parser.ParseCardUrls(html))
                    ret.Add(cardurl);
            });

            return ret.ToArray();
        }
        public void GetCardInfo(string url, int editionId)
        {
            string htmltext = GetHtml(url);

            foreach (CardWithExtraInfo cardWithExtraInfo in Parser.ParseCardInfo(htmltext))
            {
                string pictureUrl = ToAbsoluteUrl(url, cardWithExtraInfo.PictureUrl);
                int idGatherer = Parser.ExtractIdGatherer(pictureUrl);
                string baseUrl = ToAbsoluteUrl(url, string.Format("Languages.aspx?multiverseid={0}",  idGatherer));

                CardWithExtraInfo info = cardWithExtraInfo;

                ManageMultiPage(baseUrl, html => 
                    {
                        foreach (CardLanguageInfo language in Parser.ParseCardLanguage(html))
                            info.Add(language);
                    });

                InsertCardInDb(cardWithExtraInfo);
                InsertCardEditionInDb(editionId, cardWithExtraInfo, pictureUrl);
            }
        }
        private void ManageMultiPage(string baseUrl, Action<string> workOnHtml )
        {
            int page = 0;
            bool hasnextpage;
            do
            {
                hasnextpage = false;
                string realUrl = string.Format("{0}&page={1}", baseUrl, page);
                string html = GetHtml(realUrl);
                try
                {
                    workOnHtml(html);
                }
                catch (NextPageException ex)
                {
                    int index;
                    int[] pages = ex.Pages;
                    for (index = 0; index < pages.Length; index++)
                    {
                        if (page == pages[index])
                            break;
                    }

                    hasnextpage = (index + 1 < pages.Length);
                    if (hasnextpage)
                        page = pages[index + 1];
                }
            }
            while (hasnextpage);
        }
        public void InsertPictureInDb(string pictureUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(pictureUrl);

            IPicture picture = _magicDatabase.GetPicture(idGatherer);
            if (picture == null)
            {
                //No id found try insert
                byte[] pictureData = GetFile(pictureUrl);

                _magicDatabase.InsertNewPicture(idGatherer, pictureData);
            }
            //ALERT: see if we update the data
            /*
            else
            {
            }
            */
        }
        public string[] GetMissingPictureUrls()
        {
            return _magicDatabase.GetMissingPictureUrls();
        }
        public IEditionIconInfo GetEditionIcon(IDictionary<IconPageType, string> urls, string wantedEdition)
        {
            foreach (KeyValuePair<IconPageType, string> kv in urls)
            {
                IEditionFinder finder = EditionInfoFinderFactory.Instance.CreateFinder(kv.Key, GetHtml);
                if (finder == null)
                    continue;

                EditionIconInfo editionIconInfo = finder.Find(kv.Value, wantedEdition);
                if (editionIconInfo == null)
                    continue;

                string iconUrl = editionIconInfo.Url;

                if (string.IsNullOrWhiteSpace(iconUrl))
                    continue;

                byte[] editionIcon = GetFile(iconUrl);
                if (editionIcon != null && editionIcon.Length > 0)
                {
                    editionIconInfo.Icon = editionIcon;
                    return editionIconInfo;
                }
            }

            return null;
        }

        private void InsertCardEditionInDb(int idEdition, CardWithExtraInfo cardWithExtraInfo, string pictureUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(cardWithExtraInfo.PictureUrl);

            _magicDatabase.InsertNewCardEdition(idGatherer, idEdition, cardWithExtraInfo.Name, cardWithExtraInfo.PartName, cardWithExtraInfo.Rarity, pictureUrl);
        }
        public void EditionCompleted(int editionId)
        {
            _magicDatabase.EditionCompleted(editionId);
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
        private void OnNewEditionCreated(string name)
        {
            var e = NewEditionCreated;
            if (e != null)
                e(this, new EventArgs<string>(name));
        }

        private void InsertCardInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            _magicDatabase.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Text, cardWithExtraInfo.Power, cardWithExtraInfo.Toughness,
                                                        cardWithExtraInfo.CastingCost, cardWithExtraInfo.Loyalty, cardWithExtraInfo.Type, 
                                                        cardWithExtraInfo.PartName, cardWithExtraInfo.OtherPathName, cardWithExtraInfo.Languages);
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
