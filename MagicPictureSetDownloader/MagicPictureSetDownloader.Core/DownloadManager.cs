namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Common.Library.Notify;
    using Common.Web;
    using MagicPictureSetDownloader.Core.Deck;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall;

    public class DownloadManager
    {
        public event EventHandler<EventArgs<string>> NewEditionCreated;

        public const string BaseEditionUrl = @"http://gatherer.wizards.com/Pages/Default.aspx";
        private const string BaseIconUrl = @"http://gatherer.wizards.com/Handlers/Image.ashx?type=symbol&size=small&rarity={0}&set={1}";

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
                {
                    ret.Add(cardurl);
                }
            });

            return ret.ToArray();
        }
        internal void ManageMultiPage(string baseUrl, Action<string> workOnHtml)
        {
            int page = 0;
            bool hasnextpage;
            do
            {
                hasnextpage = false;
                
                string realUrl = page == 0 ? baseUrl : string.Format("{0}&page={1}", baseUrl, page);
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
                        {
                            break;
                        }
                    }

                    hasnextpage = (index + 1 < pages.Length);
                    if (hasnextpage)
                    {
                        page = pages[index + 1];
                    }
                }
            } while (hasnextpage);
        }
        public string InsertPictureInDb(string pictureUrl, object param)
        {
            string idScryFall = (string)param;

            IPicture picture = MagicDatabase.GetPicture(idScryFall);
            if (picture == null)
            {
                //No id found try insert
                byte[] pictureData = _webAccess.GetFile(pictureUrl);

                MagicDatabase.InsertNewPicture(idScryFall, pictureData);
            }

            return null;
        }
        public string InsertRuleInDb(string rulesUrl, object param)
        {
            string idScryFall = (string)param;

            string htmltext = _webAccess.GetHtml(rulesUrl);

            foreach (CardRuleInfo cardRuleInfo in Parser.ParseCardRule(htmltext))
            {
                MagicDatabase.InsertNewRuling(idScryFall, cardRuleInfo.Date, cardRuleInfo.Text);
            }

            return null;
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetRulesUrls()
        {
            return MagicDatabase.GetRulesId()
                            .Select(idScryFall => new KeyValuePair<string, object>(WebAccess.ToAbsoluteUrl(BaseEditionUrl, string.Format("Card/Details.aspx?multiverseid={0}", idScryFall)), idScryFall))
                            .ToList();
        }
        public string InsertPriceInDb(IPriceImporter priceImporter, string pricesUrl, object param)
        {
            string importErrorMessage;

            foreach (PriceInfo priceInfo in priceImporter.Parse(_webAccess, pricesUrl, param, out importErrorMessage))
            {
                MagicDatabase.InsertNewPrice(priceInfo.IdScryFall, priceInfo.UpdateDate.Date, priceInfo.PriceSource.ToString("g"), priceInfo.Foil, priceInfo.Value);
            }

            return importErrorMessage;
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetPricesUrls(IPriceImporter priceImporter)
        {
            return priceImporter.GetDefaultCardUrls(_webAccess);
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetMissingPictureUrls()
        {
            return MagicDatabase.GetMissingPictureUrls();
        }
        public byte[] GetEditionIcon(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            byte[] editionIcon = null;
            foreach (string rarity in new[] { "C", "U", "R", "M" })
            {
                try
                {
                    editionIcon = _webAccess.GetFile(string.Format(BaseIconUrl, rarity, code));
                }
                catch (WebException)
                {
                    //Manage file not found error
                }
                if (editionIcon != null && editionIcon.Length > 0)
                {
                    return editionIcon;
                }
            }

            return null;
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetPreconstructedDecksUrls(PreconstructedDeckImporter preconstructedDeckImporter)
        {
            string html = _webAccess.GetHtml(preconstructedDeckImporter.GetRootUrl());
            return preconstructedDeckImporter.GetDeckUrls(html).Select( s => new KeyValuePair<string, object>(s, null)).ToList();
        }
        public string InsertPreconstructedDeckCardsInDb(string url, PreconstructedDeckImporter preconstructedDeckImporter)
        {
            string html = _webAccess.GetHtml(url);

            DeckInfo deckInfo = preconstructedDeckImporter.ParseDeckPage(html);

            if (deckInfo == null)
            {
                return null;
            }

            MagicDatabase.InsertNewPreconstructedDeck(deckInfo.IdEdition, deckInfo.Name, url);
            IPreconstructedDeck preconstructedDeck = MagicDatabase.GetPreconstructedDeck(deckInfo.IdEdition, deckInfo.Name);

            foreach (DeckCardInfo deckCardInfo in deckInfo.Cards)
            {
                if (deckCardInfo.NeedToCreate)
                {
                    throw new Exception("Could not create");
                }
                else
                {
                    MagicDatabase.InsertOrUpdatePreconstructedDeckCardEdition(preconstructedDeck.Id, deckCardInfo.IdScryFall, deckCardInfo.Number);
                }
            }
            return null;
        }
        public string GetExtraInfo(string url)
        {
            return _webAccess.GetHtml(url);
        }
        internal void InsertCardEditionInDb(int idEdition, CardWithExtraInfo cardWithExtraInfo)
        {
            int idGatherer = Parser.ExtractIdGatherer(cardWithExtraInfo.PictureUrl);
            //ALERT: temp
            string idScryFall = idGatherer.ToString();
            MagicDatabase.InsertNewCardEdition(idScryFall, idEdition, cardWithExtraInfo.Name, cardWithExtraInfo.Rarity);
        }
        internal void InsertCardEditionVariationInDb(string idScryFall, string otherIdScryFall, string pictureUrl)
        {
            if (idScryFall != otherIdScryFall)
            {
                MagicDatabase.InsertNewCardEditionVariation(idScryFall, otherIdScryFall, pictureUrl);
            }
        }
        public void EditionCompleted(int editionId)
        {
            MagicDatabase.EditionCompleted(editionId);
        }
        private void OnNewEditionCreated(string name)
        {
            var e = NewEditionCreated;
            if (e != null)
            {
                e(this, new EventArgs<string>(name));
            }
        }
        
        internal void InsertCardInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            /* ALERT: To be review
            MagicDatabase.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Text, cardWithExtraInfo.Power, cardWithExtraInfo.Toughness,
                                        cardWithExtraInfo.CastingCost, cardWithExtraInfo.Loyalty, cardWithExtraInfo.Defense, cardWithExtraInfo.Type,
                                        cardWithExtraInfo.Languages);
                */
        }
    }
}
