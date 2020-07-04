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
        public void InsertPictureInDb(string pictureUrl)
        {
            int gathererId;
            ICardEdition cardEdition =  MagicDatabase.GetCardEditionFromPictureUrl(pictureUrl);
            if (cardEdition != null)
            {
                gathererId = cardEdition.IdGatherer;
            }
            else 
            { 
                ICardEditionVariation cardEditionVariation = MagicDatabase.GetCardEditionVariationFromPictureUrl(pictureUrl);
                if (cardEditionVariation != null)
                {
                    gathererId = cardEditionVariation.OtherIdGatherer;
                }
                else
                {
                    throw new ApplicationException("Could not find IdGatherer from url: " + pictureUrl);
                }

            }

            IPicture picture = MagicDatabase.GetPicture(gathererId);
            if (picture == null)
            {
                //No id found try insert
                byte[] pictureData = _webAccess.GetFile(pictureUrl);

                MagicDatabase.InsertNewPicture(gathererId, pictureData);
            }
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
        public void InsertPriceInDb(IPriceImporter priceImporter, string pricesUrl)
        {
            string htmltext = _webAccess.GetHtml(pricesUrl);

            foreach (PriceInfo priceInfo in priceImporter.Parse(htmltext))
            {
                MagicDatabase.InsertNewPrice(priceInfo.IdGatherer, DateTime.Today, priceImporter.PriceSource.ToString("g"), priceInfo.Foil, priceInfo.Value);
            }
        }
        public string[] GetPricesUrls(IPriceImporter priceImporter)
        {
            return priceImporter.GetUrls();
        }
        public string[] GetMissingPictureUrls()
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
        public string[] GetPreconstructedDecksUrls(PreconstructedDeckImporter preconstructedDeckImporter)
        {
            string html = _webAccess.GetHtml(preconstructedDeckImporter.GetRootUrl());
            return preconstructedDeckImporter.GetDeckUrls(html);
        }
        public void InsertPreconstructedDeckCardsInDb(string url, PreconstructedDeckImporter preconstructedDeckImporter)
        {
            string html = _webAccess.GetHtml(url);

            DeckInfo deckInfo = preconstructedDeckImporter.ParseDeckPage(html);

            if (deckInfo == null)
            {
                return;
            }

            MagicDatabase.InsertNewPreconstructedDeck(deckInfo.IdEdition, deckInfo.Name, url);
            IPreconstructedDeck preconstructedDeck = MagicDatabase.GetPreconstructedDeck(deckInfo.IdEdition, deckInfo.Name);

            foreach (DeckCardInfo deckCardInfo in deckInfo.Cards)
            {
                if (deckCardInfo.NeedToCreate)
                {
                    int idGatherer = MagicDatabase.InsertNewCardEditionWithFakeGathererId(deckCardInfo.IdEdition, deckCardInfo.IdCard, deckCardInfo.IdRarity, deckCardInfo.PictureUrl);
                    MagicDatabase.InsertOrUpdatePreconstructedDeckCardEdition(preconstructedDeck.Id, idGatherer, deckCardInfo.Number);
                }
                else
                {
                    MagicDatabase.InsertOrUpdatePreconstructedDeckCardEdition(preconstructedDeck.Id, deckCardInfo.IdGatherer, deckCardInfo.Number);
                }
            }
        }
        public string GetExtraInfo(string url)
        {
            return _webAccess.GetHtml(url);
        }
        internal void InsertCardEditionInDb(int idEdition, CardWithExtraInfo cardWithExtraInfo, string pictureUrl)
        {
            int idGatherer = Parser.ExtractIdGatherer(cardWithExtraInfo.PictureUrl);

            MagicDatabase.InsertNewCardEdition(idGatherer, idEdition, cardWithExtraInfo.Name, cardWithExtraInfo.PartName, cardWithExtraInfo.Rarity, pictureUrl);
        }
        internal void InsertCardEditionVariationInDb(int idGatherer, int otherIdGatherer, string pictureUrl)
        {
            if (idGatherer != otherIdGatherer)
            {
                MagicDatabase.InsertNewCardEditionVariation(idGatherer, otherIdGatherer, pictureUrl);
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
            MagicDatabase.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Text, cardWithExtraInfo.Power, cardWithExtraInfo.Toughness,
                                        cardWithExtraInfo.CastingCost, cardWithExtraInfo.Loyalty, cardWithExtraInfo.Type,
                                        cardWithExtraInfo.PartName, cardWithExtraInfo.OtherPathName, cardWithExtraInfo.Languages);
        }
    }
}
