namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Linq;
    using System.Threading;

    using Common.Database;
    using Common.Library.Extension;
    using Common.Library.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.DbGenerator;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        private bool _referentialLoaded;

        private readonly IList<IEdition> _editions = new List<IEdition>();
        private readonly IDictionary<string, ILanguage> _languages = new Dictionary<string, ILanguage>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, ILanguage> _alternativeNameLanguages = new Dictionary<string, ILanguage>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, IRarity> _rarities = new Dictionary<string, IRarity>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, IBlock> _blocks = new Dictionary<int, IBlock>();
        private readonly IDictionary<int, IPicture> _pictures = new Dictionary<int, IPicture>();
        private readonly IDictionary<int, IList<IPrice>> _prices = new Dictionary<int, IList<IPrice>>();
        private readonly IDictionary<string, ITreePicture> _treePictures = new Dictionary<string, ITreePicture>();
        private readonly IDictionary<string, ICard> _cards = new Dictionary<string, ICard>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, IPreconstructedDeck> _preconstructedDecks = new Dictionary<int, IPreconstructedDeck>();
        private readonly IDictionary<int, IList<IPreconstructedDeckCardEdition>> _preconstructedDeckCards = new Dictionary<int, IList<IPreconstructedDeckCardEdition>>();

        //For quicker access
        private readonly IDictionary<int, ICard> _cardsbyId = new Dictionary<int, ICard>();
        private readonly IDictionary<string, ICard> _cardsWithoutSpecialCharacters = new Dictionary<string, ICard>();

        private readonly IDictionary<int, ICardEdition> _cardEditions = new Dictionary<int, ICardEdition>();
        private readonly IDictionary<int, IList<ICardEditionVariation>> _cardEditionVariations = new Dictionary<int, IList<ICardEditionVariation>>();
        private readonly IDictionary<TypeOfOption, IList<IOption>> _allOptions = new Dictionary<TypeOfOption, IList<IOption>>();
        private int _fakeGathererId = 0;

        //Insert one new
        public void InsertNewEdition(string sourceName)
        {
            using (new WriterLock(_lock))
            {
                IEdition edition = GetEdition(sourceName);
                if (edition == null)
                {
                    Edition realEdition = new Edition { Name = sourceName, GathererName = sourceName, Completed = false, HasFoil = true };
                    AddToDbAndUpdateReferential(DatabaseType.Data, realEdition, InsertInReferential);
                }
            }
        }
        public void InsertNewEdition(string sourceName, string name, bool hasFoil, string code, int? idBlock, int? blockPosition, int? cardNumber, DateTime? releaseDate, byte[] icon)
        {
            using (new WriterLock(_lock))
            {
                IEdition edition = GetEdition(sourceName);
                if (edition == null)
                {
                    Edition realEdition = new Edition
                    {
                        Name = name,
                        GathererName = sourceName,
                        Completed = false,
                        HasFoil = hasFoil,
                        Code = code,
                        IdBlock = idBlock,
                        BlockPosition = idBlock.HasValue ? blockPosition : null,
                        CardNumber = cardNumber,
                        ReleaseDate = releaseDate
                    };
                    if (realEdition.IdBlock.HasValue)
                    {
                        realEdition.Block = _blocks.GetOrDefault(realEdition.IdBlock.Value);
                    }

                    AddToDbAndUpdateReferential(DatabaseType.Data, realEdition, InsertInReferential);
                }
                InsertNewTreePicture(name, icon);
            }
        }
        public void InsertNewPicture(int idGatherer, byte[] data)
        {
            using (new WriterLock(_lock))
            {
                if (GetPicture(idGatherer) != null || data == null || data.Length == 0)
                {
                    return;
                }

                Picture picture = new Picture { IdGatherer = idGatherer, Image = data };
                AddToDbAndUpdateReferential(DatabaseType.Picture, picture, InsertInReferential);
            }
        }
        public void InsertNewTreePicture(string name, byte[] data)
        {
            using (new WriterLock(_lock))
            {
                if (GetTreePicture(name) != null || data == null || data.Length == 0)
                {
                    return;
                }

                TreePicture treepicture = new TreePicture { Name = name, Image = data };
                AddToDbAndUpdateReferential(DatabaseType.Picture, treepicture, InsertInReferential);
            }
        }
        public void InsertNewCard(string name, string text, string power, string toughness, string castingcost, string loyalty, string type, string partName, string otherPartName, IDictionary<string, string> languages)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            using (new WriterLock(_lock))
            {
                ICard refCard = GetCard(name, partName);
                if (null == refCard)
                {
                    Card card = new Card
                    {
                        PartName = partName ?? name,
                        Name = name,
                        Text = text,
                        Power = power,
                        Toughness = toughness,
                        CastingCost = castingcost,
                        Loyalty = loyalty,
                        Type = type,
                        OtherPartName = otherPartName
                    };

                    AddToDbAndUpdateReferential(DatabaseType.Data, card, InsertInReferential);
                    refCard = card;
                }

                foreach (KeyValuePair<string, string> kv in languages)
                {
                    int idlanguage = GetLanguageId(kv.Key);
                    InsertNewTranslate(refCard, idlanguage, kv.Value);
                }
            }
        }
        public void InsertNewCardEdition(int idGatherer, int idEdition, string name, string partName, string rarity, string url)
        {
            using (new WriterLock(_lock))
            {
                int idRarity = GetRarityId(rarity);
                int idCard = GetCard(name, partName).Id;

                if (idGatherer == 0 || idEdition <= 0)
                {
                    throw new ApplicationDbException("Data are not filled correctedly");
                }

                if (GetCardEdition(idGatherer) != null)
                {
                    return;
                }

                CardEdition cardEdition = new CardEdition
                {
                    IdCard = idCard,
                    IdGatherer = idGatherer,
                    IdEdition = idEdition,
                    IdRarity = idRarity,
                    Url = url
                };

                AddToDbAndUpdateReferential(DatabaseType.Data, cardEdition, InsertInReferential);
            }
        }
        public void InsertNewCardEditionVariation(int idGatherer, int otherGathererId, string url)
        {
            using (new WriterLock(_lock))
            {
                if (idGatherer == 0 || otherGathererId <= 0)
                {
                    throw new ApplicationDbException("Data are not filled correctedly");
                }

                if (GetCardEdition(idGatherer) == null)
                {
                    return;
                }

                if (GetCardEditionVariation(idGatherer).Any(cev => cev.OtherIdGatherer == otherGathererId))
                {
                    return;
                }

                CardEditionVariation cardEditionVariation = new CardEditionVariation
                {
                    IdGatherer = idGatherer,
                    OtherIdGatherer = otherGathererId,
                    Url = url
                };

                AddToDbAndUpdateReferential(DatabaseType.Data, cardEditionVariation, InsertInReferential);
            }
        }
        public int InsertNewCardEditionWithFakeGathererId(int idEdition, int idCard, int idRarity, string url)
        {
            using (new WriterLock(_lock))
            {
                IEdition edition = _editions.FirstOrDefault(e => e.Id == idEdition);
                IRarity rarity = _rarities.Values.FirstOrDefault(r => r.Id == idRarity);
                ICard card = _cardsbyId.GetOrDefault(idCard);

                if (rarity == null || card == null || edition == null)
                {
                    throw new ApplicationDbException("Data are not filled correctedly");
                }
                if (!edition.IsNoneGatherer())
                {
                    throw new ApplicationDbException("InsertNewCardEditionWithFakeGathererId could only used for NoneGatherer edition");
                }
                int existingIdGatherer = GetIdGatherer(card, edition);
                if (existingIdGatherer != 0)
                {
                    // could have been inserted by another thread
                    return existingIdGatherer;
                }

                int idGatherer = GetNextFakeGathererId();

                CardEdition cardEdition = new CardEdition
                {
                    IdCard = idCard,
                    IdGatherer = idGatherer,
                    IdEdition = idEdition,
                    IdRarity = idRarity,
                    Url = url
                };

                AddToDbAndUpdateReferential(DatabaseType.Data, cardEdition, InsertInReferential);

                return idGatherer;
            }
        }
        public void InsertNewOption(TypeOfOption type, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ApplicationDbException("Data are not filled correctedly");
            }

            using (new WriterLock(_lock))
            {
                IOption option = GetOption(type, key);

                if (option == null)
                {
                    Option newoption = new Option { Type = type, Key = key, Value = value };
                    AddToDbAndUpdateReferential(DatabaseType.Data, newoption, InsertInReferential);
                }
                else if (option.Value != value)
                {
                    RemoveFromDbAndUpdateReferential(DatabaseType.Data, option as Option, RemoveFromReferential);

                    Option newoption = new Option { Type = type, Key = key, Value = value };
                    AddToDbAndUpdateReferential(DatabaseType.Data, newoption, InsertInReferential);
                }
            }
        }
        public void InsertNewBlock(string blockName)
        {
            using (new WriterLock(_lock))
            {
                if (GetBlock(blockName) != null)
                {
                    return;
                }

                Block block = new Block { Name = blockName };
                AddToDbAndUpdateReferential(DatabaseType.Data, block, InsertInReferential);
            }
        }
        public void InsertNewLanguage(string languageName, string alternativeName)
        {
            using (new WriterLock(_lock))
            {
                if (GetLanguage(languageName) != null)
                {
                    return;
                }

                Language language = new Language { Name = languageName, AlternativeName = alternativeName };
                AddToDbAndUpdateReferential(DatabaseType.Data, language, InsertInReferential);
            }
        }
        private void InsertNewTranslate(ICard card, int idLanguage, string name)
        {
            if (card == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                if (!card.HasTranslation(idLanguage))
                {
                    Translate translate = new Translate { IdCard = card.Id, IdLanguage = idLanguage, Name = name };
                    AddToDbAndUpdateReferential(DatabaseType.Data, translate, InsertInReferential);
                }
            }
        }
        public void InsertNewRuling(int idGatherer, DateTime addDate, string text)
        {
            ICard card = GetCard(idGatherer);
            if (card == null || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                if (!card.HasRuling(addDate, text))
                {
                    Ruling ruling = new Ruling { IdCard = card.Id, AddDate = addDate, Text = text };
                    AddToDbAndUpdateReferential(DatabaseType.Data, ruling, InsertInReferential);
                }
            }
        }
        public void InsertNewPrice(int idGatherer, DateTime addDate, string source, bool foil, int value)
        {
             if (GetCardEdition(idGatherer) == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                Price price = new Price { IdGatherer = idGatherer, AddDate = addDate, Source = source, Foil = foil, Value = value };
                AddToDbAndUpdateReferential(DatabaseType.Data, price, InsertInReferential);
            }
        }
        public void InsertNewPreconstructedDeck(int idEdition, string preconstructedDeckName, string url)
        {
            using (new WriterLock(_lock))
            {
                if (GetPreconstructedDeck(idEdition, preconstructedDeckName) != null)
                {
                    return;
                }

                PreconstructedDeck preconstructedDeck = new PreconstructedDeck { IdEdition = idEdition, Name = preconstructedDeckName, Url = url };
                AddToDbAndUpdateReferential(DatabaseType.Data, preconstructedDeck, InsertInReferential);
            }
        }
        public void InsertOrUpdatePreconstructedDeckCardEdition(int idPreconstructedDeck, int idGatherer, int count)
        {
            using (new WriterLock(_lock))
            {
                if (GetPreconstructedDeck(idPreconstructedDeck) == null)
                {
                    return;
                }

                IPreconstructedDeckCardEdition preconstructedDeckCard = GetPreconstructedDeckCard(idPreconstructedDeck, idGatherer);
                if (preconstructedDeckCard == null)
                {
                    //Insert new 
                    if (count <= 0)
                    {
                        return;
                    }

                    PreconstructedDeckCardEdition newPreconstructedDeckCardEdition = new PreconstructedDeckCardEdition
                    {
                        IdPreconstructedDeck = idPreconstructedDeck,
                        IdGatherer = idGatherer,
                        Number = count
                    };


                    AddToDbAndUpdateReferential(DatabaseType.Data, newPreconstructedDeckCardEdition, InsertInReferential);

                    return;
                }

                //Update
                if (count < 0 || count == preconstructedDeckCard.Number)
                {
                    return;
                }

                PreconstructedDeckCardEdition updatePreconstructedDeckCardEdition = preconstructedDeckCard as PreconstructedDeckCardEdition;
                if (updatePreconstructedDeckCardEdition == null)
                {
                    return;
                }

                if (count == 0)
                {
                    RemoveFromDbAndUpdateReferential(DatabaseType.Data, updatePreconstructedDeckCardEdition, RemoveFromReferential);

                    return;
                }

                updatePreconstructedDeckCardEdition.Number = count;

                using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
                {
                    Mapper<PreconstructedDeckCardEdition>.UpdateOne(cnx, updatePreconstructedDeckCardEdition);
                }
            }
        }
        private int GetNextFakeGathererId()
        {
            return Interlocked.Decrement(ref _fakeGathererId);
        }

        public void DeleteOption(TypeOfOption type, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ApplicationDbException("Data are not filled correctedly");
            }

            using (new WriterLock(_lock))
            {
                IOption option = GetOption(type, key);

                if (option == null)
                {
                    return;
                }

                RemoveFromDbAndUpdateReferential(DatabaseType.Data, option as Option, RemoveFromReferential);
            }
        }

        private void AddToDbAndUpdateReferential<T>(DatabaseType databaseType, T value, Action<T> addToReferential)
            where T : class, new()
        {
            //Lock write on calling
            if (value == null)
            {
                return;
            }

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection(databaseType))
            {
                Mapper<T>.InsertOne(cnx, value);
            }

            addToReferential(value);
        }
        private void RemoveFromDbAndUpdateReferential<T>(DatabaseType databaseType, T value, Action<T> removeFromReferential)
            where T : class, new()
        {
            //Lock write on calling
            if (value == null)
            {
                return;
            }

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection(databaseType))
            {
                Mapper<T>.DeleteOne(cnx, value);
            }

            removeFromReferential(value);
        }

        private void LoadReferentials()
        {
            //Lock Write on calling
            using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
            {
                _allOptions.Clear();
                _rarities.Clear();
                _blocks.Clear();
                _editions.Clear();
                _languages.Clear();
                _alternativeNameLanguages.Clear();
                _cards.Clear();
                _cardsbyId.Clear();
                _cardsWithoutSpecialCharacters.Clear();
                _cardEditions.Clear();
                _cardEditionVariations.Clear();
                _collections.Clear();
                _allCardInCollectionCount.Clear();
                _preconstructedDecks.Clear();
                _preconstructedDeckCards.Clear();

                foreach (Option option in Mapper<Option>.LoadAll(cnx))
                {
                    InsertInReferential(option);
                }

                foreach (Rarity rarity in Mapper<Rarity>.LoadAll(cnx))
                {
                    InsertInReferential(rarity);
                }

                foreach (Block block in Mapper<Block>.LoadAll(cnx))
                {
                    InsertInReferential(block);
                }

                foreach (Language language in Mapper<Language>.LoadAll(cnx))
                {
                    InsertInReferential(language);
                }

                foreach (Edition edition in Mapper<Edition>.LoadAll(cnx))
                {
                    if (edition.IdBlock.HasValue)
                    {
                        edition.Block = _blocks.GetOrDefault(edition.IdBlock.Value);
                    }

                    InsertInReferential(edition);
                }

                foreach (Card card in Mapper<Card>.LoadAll(cnx))
                {
                    InsertInReferential(card);
                }

                foreach (Translate translate in Mapper<Translate>.LoadAll(cnx))
                {
                    InsertInReferential(translate);
                }

                foreach (Ruling ruling in Mapper<Ruling>.LoadAll(cnx))
                {
                    InsertInReferential(ruling);
                }

                foreach (CardEdition cardEdition in Mapper<CardEdition>.LoadAll(cnx))
                {
                    InsertInReferential(cardEdition);
                }

                foreach (CardEditionVariation cardEditionVariation in Mapper<CardEditionVariation>.LoadAll(cnx))
                {
                    InsertInReferential(cardEditionVariation);
                }

                foreach (CardCollection cardCollection in Mapper<CardCollection>.LoadAll(cnx))
                {
                    InsertInReferential(cardCollection);
                }

                foreach (CardInCollectionCount cardInCollectionCount in Mapper<CardInCollectionCount>.LoadAll(cnx))
                {
                    InsertInReferential(cardInCollectionCount);
                }

                foreach (Price price in Mapper<Price>.LoadAll(cnx))
                {
                    InsertInReferential(price);
                }

                foreach (PreconstructedDeck preconstructedDeck in Mapper<PreconstructedDeck>.LoadAll(cnx))
                {
                    InsertInReferential(preconstructedDeck);
                }

                foreach (PreconstructedDeckCardEdition preconstructedDeckCardEdition in Mapper<PreconstructedDeckCardEdition>.LoadAll(cnx))
                {
                    InsertInReferential(preconstructedDeckCardEdition);
                }
            }
            using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Picture))
            {
                _treePictures.Clear();

                foreach (TreePicture treePicture in Mapper<TreePicture>.LoadAll(cnx))
                {
                    InsertInReferential(treePicture);
                }
            }

            _referentialLoaded = true;
        }
        private void InsertInReferential(IPicture picture)
        {
            _pictures.Add(picture.IdGatherer, picture);
        }
        private void InsertInReferential(ITreePicture treepicture)
        {
            _treePictures.Add(treepicture.Name, treepicture);
        }
        private void InsertInReferential(IRarity rarity)
        {
            _rarities.Add(rarity.Name, rarity);
        }
        private void InsertInReferential(IEdition edition)
        {
            _editions.Add(edition);
            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(ICard card)
        {
            string key;
            if (card.PartName == null || card.Name == card.PartName)
            {
                key = card.Name;
            }
            else
            {
                key = card.Name + card.PartName;
            }

            _cards.Add(key, card);
            if (!card.IsSplitted || card.Name.StartsWith(card.PartName))
            {
                //Remove second part of splitted card for search
                _cardsWithoutSpecialCharacters.Add(LowerCaseWithoutSpecialCharacters(card.Name), card);
            }
            _cardsbyId.Add(card.Id, card);
            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(ICardEdition cardEdition)
        {
            if (_fakeGathererId > cardEdition.IdGatherer)
            {
                _fakeGathererId = cardEdition.IdGatherer;
            }

            _cardEditions.Add(cardEdition.IdGatherer, cardEdition);
            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(ICardEditionVariation cardEditionVariation)
        {
            if (_cardEditions.GetOrDefault(cardEditionVariation.IdGatherer) == null)
            {
                throw new ApplicationDbException("Can't find CardEdition with id " + cardEditionVariation.IdGatherer);
            }

            if (!_cardEditionVariations.TryGetValue(cardEditionVariation.IdGatherer, out IList<ICardEditionVariation> variations))
            {
                variations = new List<ICardEditionVariation>();
                _cardEditionVariations.Add(cardEditionVariation.IdGatherer, variations);
            }

            variations.Add(cardEditionVariation);
            _cacheForAllDbInfos = null;
        }

        private void InsertInReferential(IOption option)
        {
            IList<IOption> options;
            if (!_allOptions.TryGetValue(option.Type, out options))
            {
                options = new List<IOption>();
                _allOptions.Add(option.Type, options);
            }

            options.Add(option);
        }
        private void InsertInReferential(Translate translate)
        {
            Card card = _cardsbyId.GetOrDefault(translate.IdCard) as Card;
            if (card == null)
            {
                throw new ApplicationDbException("Can't find card with id " + translate.IdCard);
            }

            card.AddTranslate(translate);

            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(Ruling ruling)
        {
            Card card = _cardsbyId.GetOrDefault(ruling.IdCard) as Card;
            if (card == null)
            {
                throw new ApplicationDbException("Can't find card with id " + ruling.IdCard);
            }

            card.AddRuling(ruling);

            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(Price price)
        {
            if (_cardEditions.GetOrDefault(price.IdGatherer) == null)
            {
                throw new ApplicationDbException("Can't find CardEdition with id " + price.IdGatherer);
            }

            if (!_prices.TryGetValue(price.IdGatherer, out IList<IPrice> prices))
            {
                prices = new List<IPrice>();
                _prices.Add(price.IdGatherer, prices);
            }

            prices.Add(price);

            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(ILanguage language)
        {
            _languages.Add(language.Name, language);
            if (!string.IsNullOrWhiteSpace(language.AlternativeName))
            {
                foreach (string name in language.AlternativeName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!_languages.ContainsKey(name) && !_alternativeNameLanguages.ContainsKey(name))
                    {
                        _alternativeNameLanguages.Add(name, language);
                    }
                }
            }
        }
        private void InsertInReferential(IBlock block)
        {
            _blocks.Add(block.Id, block);
        }
        private void InsertInReferential(IPreconstructedDeck preconstructedDeck)
        {
            _preconstructedDecks.Add(preconstructedDeck.Id, preconstructedDeck);
        }
        private void InsertInReferential(IPreconstructedDeckCardEdition preconstructedDeckCardEdition)
        {
            IList<IPreconstructedDeckCardEdition> list;
            if (!_preconstructedDeckCards.TryGetValue(preconstructedDeckCardEdition.IdPreconstructedDeck, out list))
            {
                list = new List<IPreconstructedDeckCardEdition>();
                _preconstructedDeckCards.Add(preconstructedDeckCardEdition.IdPreconstructedDeck, list);
            }

            if (list.Contains(preconstructedDeckCardEdition))
            {
                throw new Exception("Invalid addition");
            }

            list.Add(preconstructedDeckCardEdition);
        }

        private void RemoveFromReferential(IOption option)
        {
            IList<IOption> options = _allOptions[option.Type];
            options.Remove(option);
            if (options.Count == 0)
            {
                _allOptions.Remove(option.Type);
            }
        }
        private void RemoveFromReferential(ILanguage language)
        {
            _languages.Remove(language.Name);
            if (!string.IsNullOrWhiteSpace(language.AlternativeName))
            {
                foreach (string name in language.AlternativeName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    _alternativeNameLanguages.Remove(name);
                }
            }

        }
        private void RemoveFromReferential(IPreconstructedDeckCardEdition preconstructedDeckCardEdition)
        {
            IList<IPreconstructedDeckCardEdition> list;
            if (!_preconstructedDeckCards.TryGetValue(preconstructedDeckCardEdition.IdPreconstructedDeck, out list))
            {
                list.Remove(preconstructedDeckCardEdition);
            }
        }
        private void CheckReferentialLoaded()
        {
            if (!_referentialLoaded)
            {
                using (new WriterLock(_lock))
                {
                    if (!_referentialLoaded)
                    {
                        LoadReferentials();
                    }
                }
            }
        }

        private string LowerCaseWithoutSpecialCharacters(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            StringBuilder sb = new StringBuilder(source.Length);

            bool isPreviousSpace = false;
            foreach (char c in source)
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    isPreviousSpace = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // => a - z
                    sb.Append((char)(c + 0x20));
                    isPreviousSpace = false;
                }
                else if (c == 'æ' || c == 'Æ')
                {
                    sb.Append("ae");
                    isPreviousSpace = false;
                }
                else if (c == 'Œ' || c == 'œ')
                {
                    sb.Append("oe");
                    isPreviousSpace = false;
                }
                else if (c == 'Ñ' || c == 'ñ')
                {
                    sb.Append('n');
                    isPreviousSpace = false;
                }
                else if ((c >= 'À' && c <= 'Å') || (c >= 'à' && c <= 'å'))
                {
                    // À Á Â Ã Ä Å à á â ã ä å => a
                    sb.Append('a');
                    isPreviousSpace = false;
                }
                else if ((c >= 'È' && c <= 'Ë') || (c >= 'è' && c <= 'ë'))
                {
                    // È É Ê Ë è é ê ë => e
                    sb.Append('e');
                    isPreviousSpace = false;
                }
                else if ((c >= 'Ì' && c <= 'Ï') || (c >= 'ì' && c <= 'ï'))
                {
                    // Ì Í Î Ï ì í î ï => i
                    sb.Append('i');
                    isPreviousSpace = false;
                }
                else if ((c >= 'Ò' && c <= 'Ö') || (c >= 'ò' && c <= 'ö'))
                {
                    // Ò Ó Ô Õ Ö ò ó ô õ ö => o
                    sb.Append('o');
                    isPreviousSpace = false;
                }
                else if ((c >= 'Ù' && c <= 'Ü') || (c >= 'ù' && c <= 'ü'))
                {
                    // Ù Ú Û Ü ù ú û ü => u
                    sb.Append('u');
                    isPreviousSpace = false;
                }
                else if (c == '/')
                {
                    //Keep because manage in block
                    sb.Append(c);
                    isPreviousSpace = false;
                }
                else if (c == '\'' || c == ',' || c == '.')
                {
                    //Remove 
                }
                else if (sb.Length == 0 || isPreviousSpace)
                {
                    //Never starts with space and not two spaces
                }
                else
                {
                    sb.Append(' ');
                    isPreviousSpace = true;
                }
            }

            string ret = sb.ToString().TrimEnd();
            //Case for token
            if (ret.EndsWith(" card"))
            {
                ret = ret.Substring(0, ret.Length - 5);
            }
            foreach (string s in new []{" // ","//", " / ", "/"})
            {
                if (ret.Contains(s))
                {
                    ret = ret.Replace(s, string.Empty);
                }
            }

            return ret;
        }
    }
}
