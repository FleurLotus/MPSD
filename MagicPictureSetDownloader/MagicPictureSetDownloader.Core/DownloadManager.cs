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
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public class DownloadManager
    {
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

        public void GetAndSaveEditions()
        {
            Set[] sets = ScryFallDataRetriever.GetBulkSets(_webAccess);

            foreach (Set set in sets)
            {
                IEdition edition = MagicDatabase.GetEdition(set.Name);
                if (edition == null)
                {
                    IBlock block = GetOrAddBlock(set.Block);
                    byte[] icon = GetEditionIcon(set.IconSvgUri);

                    MagicDatabase.InsertNewEdition(set.Name, !set.NonFoilOnly, set.Code, block.Id, set.CardCount, set.ReleasedAt, icon);
                }
            }
        }
        private IBlock GetOrAddBlock(string name)
        {
            IBlock block = null;
            if (!string.IsNullOrEmpty(name))
            {
                block = MagicDatabase.GetBlock(name);
                if (block == null)
                {
                    MagicDatabase.InsertNewBlock(name);
                    block = MagicDatabase.GetBlock(name);
                }
            }
            return block;
        }
        public Card[] GetCards()
        {
            return ScryFallDataRetriever.GetCardsInfo(_webAccess, out _);
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
        public string InsertPriceInDb(IPriceImporter priceImporter, string pricesUrl, object param)
        {
            foreach (PriceInfo priceInfo in priceImporter.Parse(_webAccess, pricesUrl, param))
            {
                MagicDatabase.InsertNewPrice(priceInfo.IdScryFall, priceInfo.UpdateDate.Date, priceInfo.PriceSource.ToString("g"), priceInfo.Foil, priceInfo.Value);
            }
            return null;
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetPricesUrls(IPriceImporter priceImporter)
        {
            return priceImporter.GetDefaultCardUrls(_webAccess);
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetMissingPictureUrls()
        {
            return MagicDatabase.GetMissingPictureUrls();
        }
        private byte[] GetEditionIcon(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            byte[] editionIcon = null;
            try
            {
                editionIcon = _webAccess.GetFile(uri.ToString());
            }
            catch (WebException)
            {
                //Manage file not found error
            }
            if (editionIcon != null && editionIcon.Length > 0)
            {
                return editionIcon;
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
            MagicDatabase.InsertNewCardEdition(cardWithExtraInfo.IdScryFall, idEdition, cardWithExtraInfo.Name, cardWithExtraInfo.Rarity);
        }
        internal void InsertCardEditionVariationInDb(string idScryFall, string otherIdScryFall, string pictureUrl)
        {
            if (idScryFall != otherIdScryFall)
            {
                MagicDatabase.InsertNewCardEditionVariation(idScryFall, otherIdScryFall, pictureUrl);
            }
        }
        internal void InsertCardInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            /* ALERT: To be review InsertCardInDb
            MagicDatabase.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Text, cardWithExtraInfo.Power, cardWithExtraInfo.Toughness,
                                        cardWithExtraInfo.CastingCost, cardWithExtraInfo.Loyalty, cardWithExtraInfo.Defense, cardWithExtraInfo.Type,
                                        cardWithExtraInfo.Languages);
                */
        }
    }
}
