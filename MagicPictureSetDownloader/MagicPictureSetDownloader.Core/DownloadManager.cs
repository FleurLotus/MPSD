namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Common.Library.Notify;
    using Common.Web;
    using MagicPictureSetDownloader.Core.EditionInfos;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class DownloadManager
    {
        public event EventHandler<EventArgs<string>> NewEditionCreated;

        public const string BaseEditionUrl = @"http://gatherer.wizards.com/Pages/Default.aspx";

        private readonly WebAccess _webAccess = new WebAccess();
        private readonly Lazy<IMagicDatabaseReadAndWriteReference> _lazy = new Lazy<IMagicDatabaseReadAndWriteReference>(() => MagicDatabaseManager.ReadAndWriteReference);

        private IMagicDatabaseReadAndWriteReference MagicDatabase
        {
            get { return _lazy.Value; }
        }

        public event EventHandler<EventArgs<CredentialRequieredArgs>> CredentialRequiered
        {
            add { _webAccess.CredentialRequiered += value; }
            remove { _webAccess.CredentialRequiered -= value; }
        }

        public IEnumerable<EditionInfoWithBlock> GetEditionList(string url)
        {
            string htmltext = _webAccess.GetHtml(url);
            foreach (EditionInfo editionInfo in Parser.ParseEditionsList(htmltext))
            {
                IEdition edition = MagicDatabase.GetEdition(editionInfo.Name);
                if (edition == null)
                {
                    OnNewEditionCreated(editionInfo.Name);
                    edition = MagicDatabase.GetEdition(editionInfo.Name);
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
            string htmltext = _webAccess.GetHtml(url);

            foreach (CardWithExtraInfo cardWithExtraInfo in Parser.ParseCardInfo(htmltext))
            {
                string pictureUrl = WebAccess.ToAbsoluteUrl(url, cardWithExtraInfo.PictureUrl);
                int idGatherer = Parser.ExtractIdGatherer(pictureUrl);
                string baseUrl = WebAccess.ToAbsoluteUrl(url, string.Format("Languages.aspx?multiverseid={0}", idGatherer));

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
        private void ManageMultiPage(string baseUrl, Action<string> workOnHtml)
        {
            int page = 0;
            bool hasnextpage;
            do
            {
                hasnextpage = false;
                string realUrl = string.Format("{0}&page={1}", baseUrl, page);
                string html = _webAccess.GetHtml(realUrl);
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
            } while (hasnextpage);
        }
        public void InsertPictureInDb(string pictureUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(pictureUrl);

            IPicture picture = MagicDatabase.GetPicture(idGatherer);
            if (picture == null)
            {
                //No id found try insert
                byte[] pictureData = _webAccess.GetFile(pictureUrl);

                MagicDatabase.InsertNewPicture(idGatherer, pictureData);
            }

            //ALERT: see if we update the data
            /*
            else
            {
            }
            */
        }
        public void InsertRuleInDb(string rulesUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(rulesUrl);

            string htmltext = _webAccess.GetHtml(rulesUrl);

            foreach (CardRuleInfo cardRuleInfo in Parser.ParseCardRule(htmltext))
            {
                MagicDatabase.InsertNewRuling(idGatherer, cardRuleInfo.Date, cardRuleInfo.Text);
            }
        }
        public string[] GetRulesUrls()
        {
            return MagicDatabase.GetRulesId().Select(idGatherer => WebAccess.ToAbsoluteUrl(BaseEditionUrl, string.Format("Card/Details.aspx?multiverseid={0}", idGatherer))).ToArray();
        }

        public string[] GetMissingPictureUrls()
        {
            return MagicDatabase.GetMissingPictureUrls();
        }
        public IEditionIconInfo GetEditionIcon(IDictionary<IconPageType, string> urls, string wantedEdition)
        {
            foreach (KeyValuePair<IconPageType, string> kv in urls)
            {
                IEditionFinder finder = EditionInfoFinderFactory.Instance.CreateFinder(kv.Key, s => _webAccess.GetHtml(s));
                if (finder == null)
                    continue;

                EditionIconInfo editionIconInfo = finder.Find(kv.Value, wantedEdition);
                if (editionIconInfo == null)
                    continue;

                string iconUrl = editionIconInfo.Url;

                if (string.IsNullOrWhiteSpace(iconUrl))
                    continue;

                byte[] editionIcon = null;
                try
                {
                    editionIcon = _webAccess.GetFile(iconUrl);
                }
                catch (WebException)
                {
                    //Manage file not found error
                }
                
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

            MagicDatabase.InsertNewCardEdition(idGatherer, idEdition, cardWithExtraInfo.Name, cardWithExtraInfo.PartName, cardWithExtraInfo.Rarity, pictureUrl);
        }
        public void EditionCompleted(int editionId)
        {
            MagicDatabase.EditionCompleted(editionId);
        }

        private void OnNewEditionCreated(string name)
        {
            var e = NewEditionCreated;
            if (e != null)
                e(this, new EventArgs<string>(name));
        }

        private void InsertCardInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            MagicDatabase.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Text, cardWithExtraInfo.Power, cardWithExtraInfo.Toughness,
                                        cardWithExtraInfo.CastingCost, cardWithExtraInfo.Loyalty, cardWithExtraInfo.Type,
                                        cardWithExtraInfo.PartName, cardWithExtraInfo.OtherPathName, cardWithExtraInfo.Languages);
        }
    }
}
