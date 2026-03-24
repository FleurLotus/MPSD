namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Notify;
    using Common.Web;

    using MagicPictureSetDownloader.Core.Deck;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall;
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public class DownloadManager
    {
        private readonly WebAccess _webAccess = new WebAccess(TimeSpan.FromMinutes(5));
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

        public async Task GetAndSaveEditions(CancellationToken ct)
        {
            Set[] sets = await ScryFallDataRetriever.GetBulkSets(_webAccess, ct).ConfigureAwait(false);

            foreach (Set set in sets)
            {
                ct.ThrowIfCancellationRequested();

                IEdition edition = MagicDatabase.GetEdition(set.Name);
                if (edition == null)
                {
                    IBlock block = GetOrAddBlock(set.Block);
                    byte[] icon = null;
                    if (MagicDatabase.GetTreePicture(set.Name) == null)
                    {
                        icon = await GetEditionIcon(set.IconSvgUri, ct).ConfigureAwait(false);
                    }

                    MagicDatabase.InsertNewEdition(set.Name, !set.NonFoilOnly, set.Code.ToUpperInvariant(), block?.Id, set.CardCount, set.ReleasedAt, icon);
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
        public async IAsyncEnumerable<Card> GetCards(bool allCards, [EnumeratorCancellation] CancellationToken ct)
        {
            await foreach (Card c in ScryFallDataRetriever.GetCardsInfo(_webAccess, allCards, ct).ConfigureAwait(false))
            {
                if (!Tranformation.CardToIgnore(c))
                {
                    yield return c;
                }
            }
        }
        public async Task<string> InsertPictureInDb(string pictureUrl, object param, CancellationToken ct)
        {
            string idScryFall = (string) param;

            IPicture picture = MagicDatabase.GetPicture(idScryFall);
            if (picture == null)
            {
                //No id found try insert
                byte[] pictureData = await _webAccess.GetFileAsync(pictureUrl, ct).ConfigureAwait(false);

                MagicDatabase.InsertNewPicture(idScryFall, pictureData);
            }

            return null;
        }
        public async Task<string> InsertPriceInDb(IPriceImporter priceImporter, string pricesUrl, object param, CancellationToken ct)
        {
            await foreach (PriceInfo priceInfo in priceImporter.Parse(_webAccess, pricesUrl, param, ct).ConfigureAwait(false))
            {
                MagicDatabase.InsertNewPrice(priceInfo.IdScryFall, priceInfo.UpdateDate.Date, priceInfo.PriceSource.ToString("g"), priceInfo.Foil, priceInfo.Value);
            }
            return null;
        }
        public IAsyncEnumerable<(string url, object param)> GetPricesUrls(IPriceImporter priceImporter, CancellationToken ct)
        {
            return priceImporter.GetDefaultCardUrls(_webAccess, ct);
        }
        public async IAsyncEnumerable<(string url, object param)> GetMissingPictureUrls([EnumeratorCancellation] CancellationToken ct)
        {
            //prevents the CS1998 "async method lacks 'await'" situation) and forces the compiler to generate the async state machine required for proper async-iterator behavior.
            await Task.Yield();

            foreach (KeyValuePair<string, object> kv in MagicDatabase.GetMissingPictureUrls())
            {
                ct.ThrowIfCancellationRequested();
                yield return (kv.Key, kv.Value);
            }
        }
        private async Task<byte[]> GetEditionIcon(Uri uri, CancellationToken ct)
        {
            if (uri == null)
            {
                return null;
            }

            byte[] editionIcon = null;
            try
            {
                editionIcon = await _webAccess.GetFileAsync(uri.ToString(), ct).ConfigureAwait(false);
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
        public async IAsyncEnumerable<(string url, object param)> GetPreconstructedDecksUrls(PreconstructedDeckImporter preconstructedDeckImporter, [EnumeratorCancellation] CancellationToken ct)
        {
            string html = await _webAccess.GetHtmlAsync(preconstructedDeckImporter.GetRootUrl(), false, ct).ConfigureAwait(false);

            foreach (string s in preconstructedDeckImporter.GetDeckUrls(html))
            {
                ct.ThrowIfCancellationRequested();
                yield return (s, null);
            }
        }
        public async Task<string> InsertPreconstructedDeckCardsInDb(string url, PreconstructedDeckImporter preconstructedDeckImporter, CancellationToken ct)
        {
            string html = await _webAccess.GetHtmlAsync(url, false, ct).ConfigureAwait(false);

            DeckInfo deckInfo = preconstructedDeckImporter.ParseDeckPage(html);

            if (deckInfo == null)
            {
                return null;
            }

            MagicDatabase.InsertNewPreconstructedDeck(deckInfo.IdEdition, deckInfo.Name, url);
            IPreconstructedDeck preconstructedDeck = MagicDatabase.GetPreconstructedDeck(deckInfo.IdEdition, deckInfo.Name);

            foreach (DeckCardInfo deckCardInfo in deckInfo.Cards)
            {
                ct.ThrowIfCancellationRequested();
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
        public async Task<string> GetExtraInfo(string url, CancellationToken ct)
        {
            return await _webAccess.GetHtmlAsync(url, false, ct).ConfigureAwait(false);
        }
        internal void InsertCardInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            IEdition edition = MagicDatabase.GetEditionByCode(cardWithExtraInfo.Edition);
            string checkName = edition?.Name.ToLower();

            if (string.IsNullOrWhiteSpace(checkName) || checkName.Contains("alchemy") || checkName.Contains("online") || checkName.Contains("arena"))
            {
                return;
            }

            MagicDatabase.InsertNewCard(cardWithExtraInfo.Name, cardWithExtraInfo.Layout);

            ICard card = MagicDatabase.GetCard(cardWithExtraInfo.Name);

            foreach (CardFaceWithExtraInfo cardFaceWithExtraInfo in cardWithExtraInfo.CardFaceWithExtraInfos)
            {
                MagicDatabase.InsertNewCardFace(card.Id, cardFaceWithExtraInfo.IsMainFace, cardFaceWithExtraInfo.Name, cardFaceWithExtraInfo.Text, cardFaceWithExtraInfo.Power, cardFaceWithExtraInfo.Toughness,
                                                cardFaceWithExtraInfo.CastingCost, cardFaceWithExtraInfo.Loyalty, cardFaceWithExtraInfo.Defense, cardFaceWithExtraInfo.Type);
            }

            string url = cardWithExtraInfo.CardFaceWithExtraInfos[0].PictureUrl;
            string url2 = cardWithExtraInfo.CardFaceWithExtraInfos.Count > 1 ? cardWithExtraInfo.CardFaceWithExtraInfos[1].PictureUrl : null;
            if (url2 == url)
            {
                url2 = null;
            }

            MagicDatabase.InsertNewCardEdition(cardWithExtraInfo.IdScryFall, cardWithExtraInfo.Edition, cardWithExtraInfo.Name, cardWithExtraInfo.Rarity, url, url2, cardWithExtraInfo.FlavorName, cardWithExtraInfo.FrameEffect);

            foreach ((CardIdSource source, string id) in cardWithExtraInfo.ExternalId)
            {
                MagicDatabase.InsertNewExternalIds(cardWithExtraInfo.IdScryFall, source, id);
            }

            InsertLanguageInDb(cardWithExtraInfo);
        }
        internal void InsertLanguageInDb(CardWithExtraInfo cardWithExtraInfo)
        {
            ICard card = MagicDatabase.GetCard(cardWithExtraInfo.Name);

            if (cardWithExtraInfo.Language != null && card != null)
            {
                MagicDatabase.InsertNewTranslate(card.Id, cardWithExtraInfo.Language, cardWithExtraInfo.PrintedName ?? cardWithExtraInfo.Name);
            }
        }
    }
}