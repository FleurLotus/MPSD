namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Common.Libray;

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

        public IEnumerable<SetInfoWithBlock> GetSetList(string url)
        {
            string htmltext = GetHtml(url);
            foreach (SetInfo setInfo in Parser.ParseSetsList(htmltext))
            {
                IEdition edition = _magicDatabase.GetEdition(setInfo.Name);
                if (edition == null)
                {
                    OnNewEditionCreated(setInfo.Name);
                    edition = _magicDatabase.CreateNewEdition(setInfo.Name);
                }
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
                int idGatherer = Parser.ExtractIdGatherer(pictureUrl);

                int page = 0;
                bool hasnextpage;
                do
                {
                    hasnextpage = false;
                    string languageUrl = ToAbsoluteUrl(url, string.Format("Languages.aspx?page={0}&multiverseid={1}", page, idGatherer));
                    string html = GetHtml(languageUrl);
                    try
                    {
                        foreach (CardLanguageInfo language in Parser.ParseCardLanguage(html))
                            cardWithExtraInfo.Add(language);
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


                InsertCardInDb(cardWithExtraInfo);
                InsertCardSetInDb(editionId, cardWithExtraInfo, pictureUrl);
            }
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

        private void InsertCardSetInDb(int idEdition, CardWithExtraInfo cardWithExtraInfo, string pictureUrl)
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
        private void OnNewEditionCreated(string edition)
        {
            var e = NewEditionCreated;
            if (e != null)
                e(this, new EventArgs<string>(edition));
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
