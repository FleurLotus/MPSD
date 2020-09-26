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
    using MagicPictureSetDownloader.DbGenerator;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase : IMagicDatabaseReadAndWriteCollection,
                                           IMagicDatabaseReadAndWriteOption,
                                           IMagicDatabaseReadAndWriteCardInCollection,
                                           IMagicDatabaseReadAndUpdate

    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly DatabaseConnection _databaseConnection;
        private readonly IMultiPartCardManager _multiPartCardManager;
        //To optimize display
        private List<ICardAllDbInfo> _cacheForAllDbInfos;
        
        internal MagicDatabase(IMultiPartCardManager multiPartCardManager)
        {
            _databaseConnection = new DatabaseConnection();
            _multiPartCardManager = multiPartCardManager;
        }

        //Unitary Get
        public ICard GetCard(string name, string partName)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                string key;
                if (partName == null || partName == name)
                {
                    key = name;
                }
                else
                {
                    key = name + partName;
                }

                return _cards.GetOrDefault(key) ?? _cardsWithoutSpecialCharacters.GetOrDefault(LowerCaseWithoutSpecialCharacters(key));
            }
        }
        public IPicture GetDefaultPicture()
        {
            return GetPicture(0);
        }
        public IPicture GetPicture(int idGatherer, bool doNotCache = false)
        {
            IPicture picture;

            if (!_pictures.TryGetValue(idGatherer, out picture))
            {
                using (new WriterLock(_lock))
                {
                    if (!_pictures.TryGetValue(idGatherer, out picture))
                    {
                        picture = LoadImage(idGatherer);
                        if (picture != null && !doNotCache)
                        {
                            _pictures.Add(picture.IdGatherer, picture);
                        }
                    }
                }
            }

            return picture;
        }
        public ITreePicture GetTreePicture(string key)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                return _treePictures.GetOrDefault(key);
            }
        }
        public IEdition GetEdition(string sourceName)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                return _editions.FirstOrDefault(ed => string.Equals(ed.GathererName, sourceName, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        public IEdition GetEdition(int idGatherer)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idGatherer);
                if (cardEdition == null)
                {
                    return null;
                }

                return _editions.FirstOrDefault(e => e.Id == cardEdition.IdEdition);
            }
        }
        public ICard GetCard(int idGatherer)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idGatherer);
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
        public IList<ILanguage> GetLanguages(int idGatherer)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                ICard card = GetCard(idGatherer);
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
                return new IPreconstructedDeckCardEdition[0];
            }
        }
        private IPreconstructedDeckCardEdition GetPreconstructedDeckCard(int idPreconstructedDeck, int idGatherer)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                ICollection<IPreconstructedDeckCardEdition> preconstructedDeckCards = GetPreconstructedDeckCards(idPreconstructedDeck);
                if (preconstructedDeckCards == null)
                {
                    return null;
                }

                return preconstructedDeckCards.FirstOrDefault(pdc => pdc.IdGatherer == idGatherer);
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
            return options == null ? null : options.FirstOrDefault(o => o.Key == key);
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
                        if (collection.All(cicc => cicc.IdGatherer != cardEdition.IdGatherer))
                        {
                            continue;
                        }
                    }

                    ICardEdition edition = cardEdition;
                    ICard card = _cardsbyId.GetOrDefault(edition.IdCard);
                    cardAllDbInfo.Card = card;
                    cardAllDbInfo.Edition = _editions.FirstOrDefault(e => e.Id == edition.IdEdition);
                    cardAllDbInfo.Rarity = _rarities.Values.FirstOrDefault(r => r.Id == edition.IdRarity);
                    cardAllDbInfo.IdGatherer = cardEdition.IdGatherer;
                    cardAllDbInfo.IdGathererPart2 = 0;
                    IList<IPrice> prices = _prices.GetOrDefault(cardEdition.IdGatherer);
                    cardAllDbInfo.Prices = prices == null ? new List<IPrice>() : new List<IPrice>(prices);
                    cardAllDbInfo.SetStatistics(GetCardCollectionStatistics(card));
                    IList<ICardEditionVariation> other;
                    if (_cardEditionVariations.TryGetValue(cardEdition.IdGatherer, out other))
                    {
                        cardAllDbInfo.VariationIdGatherers = other.Select(cev => cev.OtherIdGatherer).ToArray();
                    }
                    else
                    {
                        cardAllDbInfo.VariationIdGatherers = new int[0];
                    }

                    cardAllDbInfo.VariationIdGatherers2 = new int[0];

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
                            IList<ICardEditionVariation> other2;
                            if (_cardEditionVariations.TryGetValue(cardEdition2.IdGatherer, out other2))
                            {
                                cardAllDbInfo.VariationIdGatherers2 = other2.Select(cev => cev.OtherIdGatherer).ToArray();
                            }
                        }
                    }

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
                IList<IOption> options;
                if (!_allOptions.TryGetValue(type, out options))
                {
                    return null;
                }

                return new List<IOption>(options).AsReadOnly();
            }
        }

        private ICardEdition GetCardEdition(int idGatherer)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                return _cardEditions.GetOrDefault(idGatherer);
            }
        }
        public IList<ICardEditionVariation> GetCardEditionVariation(int idGatherer)
        {
            CheckReferentialLoaded();

            using (new ReaderLock(_lock))
            {
                IList<ICardEditionVariation> variations = _cardEditionVariations.GetOrDefault(idGatherer);
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
                ILanguage lang;
                if (_languages.TryGetValue(language, out lang) && lang != null)
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
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
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

        public ICardEdition GetCardEditionFromPictureUrl(string pictureUrl)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _cardEditions.Values.FirstOrDefault(ce => ce.Url == pictureUrl);
            }
        }
        public ICardEditionVariation GetCardEditionVariationFromPictureUrl(string pictureUrl)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _cardEditionVariations.Values.SelectMany(cevs => cevs).FirstOrDefault(cev => cev.Url == pictureUrl);
            }
        }

        private ICollection<int> GetAllPicturesId()
        {
            using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Picture))
            {
                return Mapper<PictureKey>.LoadAll(cnx).Select(pk => pk.IdGatherer).ToList();
            }
        }

        public void EditionCompleted(int editionId)
        {
            using (new WriterLock(_lock))
            {
                Edition newEdition = _editions.FirstOrDefault(e => e.Id == editionId) as Edition;
                if (newEdition == null || newEdition.Completed)
                {
                    return;
                }

                newEdition.Completed = true;

                using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
                {
                    Mapper<Edition>.UpdateOne(cnx, newEdition);
                }
            }
        }
        public string[] GetMissingPictureUrls()
        {
            HashSet<int> idGatherers = new HashSet<int>(GetAllPicturesId());

            return AllCardEditions().Where(ce => !string.IsNullOrWhiteSpace(ce.Url) && !idGatherers.Contains(ce.IdGatherer)).Select(ce => ce.Url)
                          .Union(AllCardEditionVariations().Where(cev => !string.IsNullOrWhiteSpace(cev.Url) && !idGatherers.Contains(cev.OtherIdGatherer)).Select(cev => cev.Url))
                          .ToArray();
        }
        public ICardAllDbInfo[] GetCardsWithPicture()
        {
            HashSet<int> idGatherers = new HashSet<int>(GetAllPicturesId());

            return GetAllInfos().Where(ce => idGatherers.Contains(ce.IdGatherer) || idGatherers.Contains(ce.IdGathererPart2)).ToArray();
        }
        public int[] GetRulesId()
        {
            using (new ReaderLock(_lock))
            {
                IDictionary<int, ICardEdition> temp = new Dictionary<int, ICardEdition>();
                foreach (ICardEdition ce in AllCardEditions())
                {
                    if (!temp.ContainsKey(ce.IdCard))
                    {
                        temp.Add(ce.IdCard, ce);
                    }
                }

                return temp.Values.Select(ce => ce.IdGatherer).Where(id => id > 0).ToArray();
            }
        }

        private IPicture LoadImage(int idGatherer)
        {
            Picture picture = new Picture { IdGatherer = idGatherer };

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Picture))
            {
                return Mapper<Picture>.Load(cnx, picture);
            }
        }
    }
}
