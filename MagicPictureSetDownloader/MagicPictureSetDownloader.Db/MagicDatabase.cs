namespace MagicPictureSetDownloader.Db
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;

    using Common.Collection;
    using Common.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase : IMagicDatabaseReadAndWriteCollectionInBatch,
                                           IMagicDatabaseReadAndWriteOption,
                                           IMagicDatabaseReadAndWriteCardInCollectionInBatch,
                                           IMagicDatabaseReadAndUpdate

    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly DatabaseConnection _databaseConnection;
        private readonly PictureDatabase _pictureDatabase;
        //To optimize display
        private List<ICardAllDbInfo> _cacheForAllDbInfos;

        internal MagicDatabase()
        {
            _databaseConnection = new DatabaseConnection();
            _pictureDatabase = new PictureDatabase();
        }

        //Unitary Get
        public ICard GetCard(string name)
        {
            using (new ReaderLock(_lock))
            {
                return GetCardRead(name);
            }
        }

        public IPicture GetDefaultPicture()
        {
            return GetPicture("00000000-0000-0000-0000-000000000000");
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
            using (new ReaderLock(_lock))
            {
                return GetEditionRead(sourceName);
            }
        }
        public IEdition GetEditionByCode(string code)
        {
            using (new ReaderLock(_lock))
            {
                return GetEditionByCodeRead(code);
            }
        }
        public IEdition GetEditionByIdScryFall(string idScryFall)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEditionRead(idScryFall);
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
                return GetCardByIdScryFallRead(idScryFall);
            }
        }
        public ILanguage GetLanguage(int idLanguage)
        {
            using (new ReaderLock(_lock))
            {
                return _languages.Values.FirstOrDefault(l => l.Id == idLanguage);
            }
        }
        public IBlock GetBlock(string blockName)
        {
            using (new ReaderLock(_lock))
            {
                return GetBlockRead(blockName);
            }
        }
        public ILanguage GetDefaultLanguage()
        {
            using (new ReaderLock(_lock))
            {
                return GetLanguageRead(Constants.Unknown);
            }
        }
        public ILanguage GetEnglishLanguage()
        {
            using (new ReaderLock(_lock))
            {
                return GetLanguageRead(Constants.English);
            }
        }
        public IList<ILanguage> GetLanguages(string idScryFall)
        {
            using (new ReaderLock(_lock))
            {
                ICard card = GetCardByIdScryFallRead(idScryFall);
                if (card == null)
                {
                    return null;
                }

                IList<ILanguage> languages = new List<ILanguage> { GetLanguageRead(Constants.Unknown) };
                foreach (ILanguage language in _languages.Values.Where(l => !languages.Contains(l) && card.HasTranslation(l.Id)))
                {
                    languages.Add(language);
                }

                return languages;
            }
        }
        public IPreconstructedDeck GetPreconstructedDeck(int? idEdition, string preconstructedDeckName)
        {
            using (new ReaderLock(_lock))
            {
                return GetPreconstructedDeckRead(idEdition, preconstructedDeckName);
            }
        }
        public ICollection<IPreconstructedDeckCardEdition> GetPreconstructedDeckCards(IPreconstructedDeck preconstructedDeck)
        {
            using (new ReaderLock(_lock))
            {
                return GetPreconstructedDeckCardsRead(preconstructedDeck?.Id);
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
            using (new ReaderLock(_lock))
            {
                ICollection<ICardInCollectionCount> collection = null;
                if (onlyInCollectionId != -1)
                {
                    collection = GetCardCollectionRead(onlyInCollectionId);
                }

                if (collection == null && _cacheForAllDbInfos != null)
                {
                    //No filter and no change since last call but recalculate statistics 
                    foreach (CardAllDbInfo cardAllDbInfo in _cacheForAllDbInfos.Cast<CardAllDbInfo>())
                    {
                        cardAllDbInfo.SetStatistics(GetCardCollectionStatisticsRead(cardAllDbInfo.Card));
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
                    cardAllDbInfo.FrameEffect = cardEdition.FrameEffect;
                    IList<IPrice> prices = _prices.GetOrDefault(cardEdition.IdScryFall);
                    cardAllDbInfo.Prices = prices == null ? new List<IPrice>() : new List<IPrice>(prices);
                    cardAllDbInfo.SetStatistics(GetCardCollectionStatisticsRead(card));

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
            using (new ReaderLock(_lock))
            {
                if (!_allOptions.TryGetValue(type, out IList<IOption> options))
                {
                    return null;
                }

                return new List<IOption>(options).AsReadOnly();
            }
        }

        public ICollection<IRarity> GetAllRarities()
        {
            using (new ReaderLock(_lock))
            {
                return new List<IRarity>(_rarities.Values).ToArray();
            }
        }
        public ICollection<IEdition> GetAllEditions()
        {
            using (new ReaderLock(_lock))
            {
                return new List<IEdition>(_editions).AsReadOnly();
            }
        }
        public ICollection<IBlock> GetAllBlocks()
        {
            using (new ReaderLock(_lock))
            {
                return new List<IBlock>(_blocks.Values).AsReadOnly();
            }
        }
        public ICollection<ILanguage> GetAllLanguages()
        {
            using (new ReaderLock(_lock))
            {
                return new List<ILanguage>(_languages.Values).AsReadOnly();
            }
        }
        public ICollection<IPreconstructedDeck> GetAllPreconstructedDecks()
        {
            using (new ReaderLock(_lock))
            {
                return new List<IPreconstructedDeck>(_preconstructedDecks.Values).AsReadOnly();
            }
        }

        private ICollection<ICardEdition> AllCardEditions()
        {
            using (new ReaderLock(_lock))
            {
                return new List<ICardEdition>(_cardEditions.Values).AsReadOnly();
            }
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetMissingPictureUrls()
        {
            HashSet<string> scryFallIds = new HashSet<string>(_pictureDatabase.GetAllPicturesGuid());

            return AllCardEditions().Where(ce => !string.IsNullOrWhiteSpace(ce.Url) && !scryFallIds.Contains(ce.IdScryFall))
                                        .Select(ce => new KeyValuePair<string, object>(ce.Url, ce.IdScryFall))
                .Union(AllCardEditions().Where(ce => !string.IsNullOrWhiteSpace(ce.Url2) && !scryFallIds.Contains(ce.IdScryFall + GetVersoExtension()))
                                        .Select(ce => new KeyValuePair<string, object>(ce.Url2, ce.IdScryFall + GetVersoExtension()))).ToList();
        }
        public string GetVersoExtension()
        {
            return "-2";
        }
    }
}