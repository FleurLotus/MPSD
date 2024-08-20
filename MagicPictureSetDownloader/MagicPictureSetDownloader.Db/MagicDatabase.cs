namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using Common.Database;
    using Common.Library.Extension;
    using Common.Library.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase : IMagicDatabaseReadAndWriteCollection,
                                           IMagicDatabaseReadAndWriteOption,
                                           IMagicDatabaseReadAndWriteCardInCollection,
                                           IMagicDatabaseReadAndUpdate

    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly DatabaseConnection _databaseConnection;
        private readonly PictureDatabase _pictureDatabase;
        private readonly IMultiPartCardManager _multiPartCardManager;
        //To optimize display
        private List<ICardAllDbInfo> _cacheForAllDbInfos;
        
        internal MagicDatabase(IMultiPartCardManager multiPartCardManager)
        {
            _databaseConnection = new DatabaseConnection();
            _pictureDatabase = new PictureDatabase();
            _multiPartCardManager = multiPartCardManager;
        }

        //Unitary Get
        public ICard GetCard(string name)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _cards.GetOrDefault(name);
            }
        }
        //Unitary Get
        public ICardFace GetCardFace(string idScryFall)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _cardFaces.GetOrDefault(idScryFall);
            }
        }

        public IPicture GetDefaultPicture()
        {
            return GetPicture("");
        }
        public IPicture GetPicture(string idScryFall, bool doNotCache = false)
        {
            return _pictureDatabase.GetPicture(idScryFall, doNotCache);
        }
        public ITreePicture GetTreePicture(string key)
        {
            return _pictureDatabase.GetTreePicture(key);
        }
        public IEdition GetEdition(string sourceName)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                return _editions.FirstOrDefault(ed => string.Equals(ed.Name, sourceName, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        public IEdition GetEditionByIdScryFall(string idScryFall)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idScryFall);
                if (cardEdition == null)
                {
                    return null;
                }

                return _editions.FirstOrDefault(e => e.Id == cardEdition.IdEdition);
            }
        }
        public ICard GetCardByIdScryFall(string idScryFall)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idScryFall);
                if (cardEdition == null)
                {
                    return null;
                }

                return _cardsbyId.GetOrDefault(cardEdition.IdCard);
            }
        }
        public ILanguage GetLanguage(int idLanguage)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                return _languages.Values.FirstOrDefault(l => l.Id == idLanguage);
            }
        }
        public IBlock GetBlock(string blockName)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                return _blocks.Values.FirstOrDefault(b => string.Compare(b.Name, blockName, StringComparison.InvariantCultureIgnoreCase) == 0);
            }
        }
        public ILanguage GetDefaultLanguage()
        {
            return GetLanguage(Constants.Unknown);
        }
        public ILanguage GetEnglishLanguage()
        {
            return GetLanguage(Constants.English);
        }
        public IList<ILanguage> GetLanguages(string idScryFall)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                ICard card = GetCardByIdScryFall(idScryFall);
                if (card == null)
                {
                    return null;
                }

                IList<ILanguage> languages = new List<ILanguage> { GetDefaultLanguage() };
                foreach (ILanguage language in _languages.Values.Where(l => !languages.Contains(l) && card.HasTranslation(l.Id)))
                {
                    languages.Add(language);
                }

                return languages;
            }
        }
        public IPreconstructedDeck GetPreconstructedDeck(int idEdition, string preconstructedDeckName)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _preconstructedDecks.Values.FirstOrDefault(pd => pd.IdEdition == idEdition && string.Compare(pd.Name, preconstructedDeckName, StringComparison.InvariantCultureIgnoreCase) == 0);
            }
        }
        public ICollection<IPreconstructedDeckCardEdition> GetPreconstructedDeckCards(IPreconstructedDeck preconstructedDeck)
        {
            if (preconstructedDeck == null)
            {
                return null;
            }

            return GetPreconstructedDeckCards(preconstructedDeck.Id);
        }
        private ICollection<IPreconstructedDeckCardEdition> GetPreconstructedDeckCards(int idPreconstructedDeck)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                if (_preconstructedDeckCards.TryGetValue(idPreconstructedDeck, out IList<IPreconstructedDeckCardEdition> preconstructedDeckCards))
                {
                    return preconstructedDeckCards.ToArray();
                }
                return Array.Empty<IPreconstructedDeckCardEdition>();
            }
        }
        private IPreconstructedDeckCardEdition GetPreconstructedDeckCard(int idPreconstructedDeck, string idScryFall)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                ICollection<IPreconstructedDeckCardEdition> preconstructedDeckCards = GetPreconstructedDeckCards(idPreconstructedDeck);
                if (preconstructedDeckCards == null)
                {
                    return null;
                }

                return preconstructedDeckCards.FirstOrDefault(pdc => pdc.IdScryFall == idScryFall);
            }
        }
        public IPreconstructedDeck GetPreconstructedDeck(int idPreconstructedDeck)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                if (_preconstructedDecks.TryGetValue(idPreconstructedDeck, out IPreconstructedDeck preconstructedDeck))
                {
                    return preconstructedDeck;
                }
                return null;
            }
        }

        public IOption GetOption(TypeOfOption type, string key)
        {
            IList<IOption> options = GetOptions(type);
            return options?.FirstOrDefault(o => o.Key == key);
        }

        //Ensembly Get
        public ICollection<ICardAllDbInfo> GetAllInfos(int onlyInCollectionId = -1)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                ICollection<ICardInCollectionCount> collection = null;
                if (onlyInCollectionId != -1)
                {
                    collection = GetCardCollection(onlyInCollectionId);
                }

                if (collection == null && _cacheForAllDbInfos != null)
                {
                    //No filter and no change since last call but recalculate statistics 
                    foreach (CardAllDbInfo cardAllDbInfo in _cacheForAllDbInfos.Cast<CardAllDbInfo>())
                    {
                        cardAllDbInfo.SetStatistics(GetCardCollectionStatistics(cardAllDbInfo.Card));
                    }

                    return _cacheForAllDbInfos.AsReadOnly();
                }

                List<ICardAllDbInfo> ret = new List<ICardAllDbInfo>();
                foreach (ICardEdition cardEdition in _cardEditions.Values)
                {
                    CardAllDbInfo cardAllDbInfo = new CardAllDbInfo();
                    if (collection != null)
                    {
                        if (collection.All(cicc => cicc.IdScryFall != cardEdition.IdScryFall))
                        {
                            continue;
                        }
                    }

                    ICardEdition edition = cardEdition;
                    ICard card = _cardsbyId.GetOrDefault(edition.IdCard);
                    cardAllDbInfo.Card = card;
                    cardAllDbInfo.Edition = _editions.FirstOrDefault(e => e.Id == edition.IdEdition);
                    cardAllDbInfo.Rarity = _rarities.Values.FirstOrDefault(r => r.Id == edition.IdRarity);
                    cardAllDbInfo.IdScryFall = cardEdition.IdScryFall;
                    IList<IPrice> prices = _prices.GetOrDefault(cardEdition.IdScryFall);
                    cardAllDbInfo.Prices = prices == null ? new List<IPrice>() : new List<IPrice>(prices);
                    cardAllDbInfo.SetStatistics(GetCardCollectionStatistics(card));
                    if (_cardEditionVariations.TryGetValue(cardEdition.IdScryFall, out IList<ICardEditionVariation> other))
                    {
                        cardAllDbInfo.VariationIdScryFalls = other.Select(cev => cev.OtherIdScryFall).ToArray();
                    }
                    else
                    {
                        cardAllDbInfo.VariationIdScryFalls = Array.Empty<string>();
                    }
                    ICardFace[] cardFaces = card.CardFaceIds.Select(id => _cardFacesbyId.GetOrDefault(id)).ToArray();
                    cardAllDbInfo.CardFaces = cardFaces;
                    cardAllDbInfo.MainCardFace = cardFaces[0];

                    /* ALERT to review For Multipart card
                    //For Multipart card
                    if (_multiPartCardManager.HasMultiPart(card))
                    {
                        //This is the reverse side of a recto-verso card no need to do anything
                        if (_multiPartCardManager.ShouldIgnore(card))
                        {
                            continue;
                        }
                        ICard cardPart2 = _multiPartCardManager.GetOtherPartCard(card, GetCard);
                        cardAllDbInfo.CardPart2 = cardPart2;

                        //Be sure to get the other part (Up/Down, Splitted and Adventure have the same gatherer id so no return)
                        ICardEdition cardEdition2 = _cardEditions.Values.FirstOrDefault(ce => ce.IdEdition == edition.IdEdition && ce.IdCard == cardPart2.Id && ce.IdGatherer != edition.IdGatherer);

                        //Verso of Reserse Card and Multi-card
                        if (cardEdition2 != null)
                        {
                            cardAllDbInfo.IdGathererPart2 = cardEdition2.IdGatherer;
                            if (_cardEditionVariations.TryGetValue(cardEdition2.IdGatherer, out IList<ICardEditionVariation> other2))
                            {
                                cardAllDbInfo.VariationIdGatherers2 = other2.Select(cev => cev.OtherIdGatherer).ToArray();
                            }
                        }
                    }
                    */
                    ret.Add(cardAllDbInfo);
                }

                //Push in cache in no filter
                if (collection == null)
                {
                    _cacheForAllDbInfos = ret;
                }

                return ret.AsReadOnly();
            }
        }
        public IList<IOption> GetOptions(TypeOfOption type)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                if (!_allOptions.TryGetValue(type, out IList<IOption> options))
                {
                    return null;
                }

                return new List<IOption>(options).AsReadOnly();
            }
        }

        private ICardEdition GetCardEdition(string idScryFall)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                return _cardEditions.GetOrDefault(idScryFall);
            }
        }
        public IList<ICardEditionVariation> GetCardEditionVariation(string idScryFall)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                IList<ICardEditionVariation> variations = _cardEditionVariations.GetOrDefault(idScryFall);
                if (variations == null)
                {
                    return new List<ICardEditionVariation>().AsReadOnly();
                }

                return new List<ICardEditionVariation>(variations).AsReadOnly();
            }
        }

        public IRarity GetRarity(string rarity)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _rarities.GetOrDefault(rarity);
            }
        }
        private int GetRarityId(string rarity)
        {
            return GetRarity(rarity).Id;
        }
        private int GetLanguageId(string language)
        {
            return GetLanguage(language).Id;
        }
        private ILanguage GetLanguage(string language)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                if (_languages.TryGetValue(language, out ILanguage lang) && lang != null)
                {
                    return lang;
                }

                if (_alternativeNameLanguages.TryGetValue(language, out lang) && lang != null)
                {
                    return lang;
                }

                return null;
            }
        }

        public ICollection<IEdition> GetAllEditions()
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return new List<IEdition>(_editions).AsReadOnly();
            }
        }
        public ICollection<IBlock> GetAllBlocks()
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return new List<IBlock>(_blocks.Values).AsReadOnly();
            }
        }
        public ICollection<ILanguage> GetAllLanguages()
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return new List<ILanguage>(_languages.Values).AsReadOnly();
            }
        }
        public ICollection<IPreconstructedDeck> GetAllPreconstructedDecks()
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return new List<IPreconstructedDeck>(_preconstructedDecks.Values).AsReadOnly();
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private ICollection<IRarity> AllRarities()
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return new List<IRarity>(_rarities.Values).AsReadOnly();
            }
        }
        private ICollection<ICardEdition> AllCardEditions()
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return new List<ICardEdition>(_cardEditions.Values).AsReadOnly();
            }
        }
        private ICollection<ICardEditionVariation> AllCardEditionVariations()
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return new List<ICardEditionVariation>(_cardEditionVariations.SelectMany(kv => kv.Value)).AsReadOnly();
            }
        }

        public IReadOnlyList<KeyValuePair<string, object>> GetMissingPictureUrls()
        {
            HashSet<int> idGatherers = new HashSet<int>(_pictureDatabase.GetAllPictureIds());

            /* ALERT: TO REVIEW GetMissingPictureUrls
            return AllCardEditions().Where(ce => !string.IsNullOrWhiteSpace(ce.Url) && !idGatherers.Contains(ce.IdScryFall))
                                        .Select(ce => new KeyValuePair<string, object>(ce.Url, ce.IdGatherer))
                          .Union(AllCardEditionVariations().Where(cev => !string.IsNullOrWhiteSpace(cev.Url) && !idGatherers.Contains(cev.OtherIdGatherer))
                                        .Select(cev => new KeyValuePair<string, object>(cev.Url, cev.OtherIdGatherer))).ToList();
            */
            return Array.Empty<KeyValuePair<string, object>>();
        }
    }
}
