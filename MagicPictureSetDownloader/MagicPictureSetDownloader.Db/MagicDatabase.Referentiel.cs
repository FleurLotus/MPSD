namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Linq;
    
    using Common.Database;
    using Common.Library.Extension;
    using Common.Library.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;
    
    internal partial class MagicDatabase
    {
        private bool _referentialLoaded;

        private readonly IList<IEdition> _editions = new List<IEdition>();
        private readonly IDictionary<string, ILanguage> _languages = new Dictionary<string, ILanguage>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, ILanguage> _alternativeNameLanguages = new Dictionary<string, ILanguage>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, IRarity> _rarities = new Dictionary<string, IRarity>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, IBlock> _blocks = new Dictionary<int, IBlock>();
        private readonly IDictionary<string, ICard> _cards = new Dictionary<string, ICard>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, ICardFace> _cardFaces = new Dictionary<int, ICardFace>();
        private readonly IDictionary<string, IList<IPrice>> _prices = new Dictionary<string, IList<IPrice>>();
        private readonly IDictionary<int, IPreconstructedDeck> _preconstructedDecks = new Dictionary<int, IPreconstructedDeck>();
        private readonly IDictionary<int, IList<IPreconstructedDeckCardEdition>> _preconstructedDeckCards = new Dictionary<int, IList<IPreconstructedDeckCardEdition>>();

        //For quicker access
        private readonly IDictionary<int, ICard> _cardsbyId = new Dictionary<int, ICard>();
        private readonly IDictionary<string, ICard> _cardsWithoutSpecialCharacters = new Dictionary<string, ICard>();

        private readonly IDictionary<string, ICardEdition> _cardEditions = new Dictionary<string, ICardEdition>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<TypeOfOption, IList<IOption>> _allOptions = new Dictionary<TypeOfOption, IList<IOption>>();

        public void InsertNewEdition(string sourceName, bool hasFoil, string code, int? idBlock, int? cardNumber, DateTime? releaseDate, byte[] icon)
        {
            using (new WriterLock(_lock))
            {
                IEdition edition = GetEdition(sourceName);
                if (edition == null)
                {
                    Edition realEdition = new Edition
                    {
                        Name = sourceName,
                        HasFoil = hasFoil,
                        Code = code,
                        IdBlock = idBlock,
                        CardNumber = cardNumber,
                        ReleaseDate = releaseDate
                    };
                    if (realEdition.IdBlock.HasValue)
                    {
                        realEdition.Block = _blocks.GetOrDefault(realEdition.IdBlock.Value);
                    }

                    AddToDbAndUpdateReferential(realEdition, InsertInReferential);
                }
                
                InsertNewTreePicture(sourceName, icon, true);
            }
        }
        public void InsertNewPicture(string idScryFall, byte[] data)
        {
            _pictureDatabase.InsertNewPicture(idScryFall, data);
        }
        public void InsertNewTreePicture(string name, byte[] data, bool isSvg)
        {
            _pictureDatabase.InsertNewTreePicture(name, data, isSvg);
        }
        public void InsertNewCard(string name, string layout)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            using (new WriterLock(_lock))
            {
                ICard card = GetCard(name);
                if (null == card)
                {
                    Card c = new Card { Name = name, Layout = layout };
                    AddToDbAndUpdateReferential(c, InsertInReferential);
                }
            }
        }
        public void InsertNewCardFace(int idCard, bool isMainFace, string name, string text, string power, string toughness, string castingcost, string loyalty, string defense, string type, string url)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (new WriterLock(_lock))
            {
                ICard refCard = _cardsbyId.GetOrDefault(idCard);
                if (null != refCard)
                {
                    ICardFace cardface = GetCardFace(idCard, name);
                    if (cardface == null)
                    {
                        CardFace cardFace = new CardFace
                        {
                            Name = name,
                            Text = text,
                            Power = power,
                            Toughness = toughness,
                            CastingCost = castingcost,
                            Loyalty = loyalty,
                            Defense = defense,
                            Type = type,
                            Url = url,
                            IdCard = idCard,
                            IsMainFace = isMainFace
                        };

                        InsertNewCardFace(refCard, cardFace);
                    }
                }
            }
        }
        private void InsertNewCardFace(ICard card, ICardFace cardFace)
        {
            if (card == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                if (!card.HasCardFace(cardFace.Name))
                {
                    AddToDbAndUpdateReferential((CardFace)cardFace, InsertInReferential);
                }
            }
        }
        public void InsertNewExternalIds(string idScryFall, CardIdSource cardIdSource, string externalId)
        {
            if (string.IsNullOrWhiteSpace(idScryFall))
            {
                throw new ArgumentNullException(nameof(idScryFall));
            }
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentNullException(nameof(externalId));
            }

            using (new WriterLock(_lock))
            {
                ICardEdition cd = GetCardEdition(idScryFall);
                IReadOnlyDictionary<CardIdSource, IReadOnlyList<string>> allExternalIds = cd.ExternalId;

                if (allExternalIds.TryGetValue(cardIdSource, out IReadOnlyList<string> externalIdList))
                {
                    if (externalIdList.Contains(externalId))
                    {
                        return;
                    }
                }

                ExternalIds externalIds = new ExternalIds
                {
                    IdScryFall = idScryFall,
                    ExternalId = externalId,
                    CardIdSource = cardIdSource.ToString()
                };
                AddToDbAndUpdateReferential(externalIds, InsertInReferential);
            }
        }

        public void InsertNewCardEdition(string idScryFall, string editionCode, string name, string rarity)
        {
            using (new WriterLock(_lock))
            {
                int idRarity = GetRarityId(rarity);
                int idCard = GetCard(name).Id;
                int idEdition = GetEditionByCode(editionCode).Id;

                if (string.IsNullOrEmpty(idScryFall))
                {
                    throw new ApplicationDbException("Data are not filled correctedly");
                }

                if (GetCardEdition(idScryFall) != null)
                {
                    return;
                }

                CardEdition cardEdition = new CardEdition
                {
                    IdCard = idCard,
                    IdScryFall = idScryFall,
                    IdEdition = idEdition,
                    IdRarity = idRarity,
                };

                AddToDbAndUpdateReferential(cardEdition, InsertInReferential);
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
                    AddToDbAndUpdateReferential(newoption, InsertInReferential);
                }
                else if (option.Value != value)
                {
                    RemoveFromDbAndUpdateReferential(option as Option, RemoveFromReferential);

                    Option newoption = new Option { Type = type, Key = key, Value = value };
                    AddToDbAndUpdateReferential(newoption, InsertInReferential);
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
                AddToDbAndUpdateReferential(block, InsertInReferential);
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
                AddToDbAndUpdateReferential(language, InsertInReferential);
            }
        }
        public void InsertNewTranslate(int idCard, string language, string name)
        {
            ICard refCard = _cardsbyId.GetOrDefault(idCard);

            if (refCard == null)
            {
                return;
            }

            int idLanguage = GetLanguage(language).Id;

            using (new WriterLock(_lock))
            {
                if (!refCard.HasTranslation(idLanguage))
                {
                    Translate translate = new Translate { IdCard = idCard, IdLanguage = idLanguage, Name = name };
                    AddToDbAndUpdateReferential(translate, InsertInReferential);
                }
            }
        }
        public void InsertNewPrice(string idScryFall, DateTime addDate, string source, bool foil, int value)
        {
            if (GetCardEdition(idScryFall) == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                Price price = new Price { IdScryFall = idScryFall, AddDate = addDate, Source = source, Foil = foil, Value = value };
                AddToDbAndUpdateReferential(price, InsertInReferential);
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
                AddToDbAndUpdateReferential(preconstructedDeck, InsertInReferential);
            }
        }
        public void InsertOrUpdatePreconstructedDeckCardEdition(int idPreconstructedDeck, string idScryFall, int count)
        {
            using (new WriterLock(_lock))
            {
                if (GetPreconstructedDeck(idPreconstructedDeck) == null)
                {
                    return;
                }

                IPreconstructedDeckCardEdition preconstructedDeckCard = GetPreconstructedDeckCard(idPreconstructedDeck, idScryFall);
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
                        IdScryFall = idScryFall,
                        Number = count
                    };


                    AddToDbAndUpdateReferential(newPreconstructedDeckCardEdition, InsertInReferential);

                    return;
                }

                //Update
                if (count < 0 || count == preconstructedDeckCard.Number)
                {
                    return;
                }

                if (preconstructedDeckCard is not PreconstructedDeckCardEdition updatePreconstructedDeckCardEdition)
                {
                    return;
                }

                if (count == 0)
                {
                    RemoveFromDbAndUpdateReferential(updatePreconstructedDeckCardEdition, RemoveFromReferential);

                    return;
                }

                updatePreconstructedDeckCardEdition.Number = count;

                using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
                {
                    Mapper<PreconstructedDeckCardEdition>.UpdateOne(cnx, updatePreconstructedDeckCardEdition);
                }
            }
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

                RemoveFromDbAndUpdateReferential(option as Option, RemoveFromReferential);
            }
        }

        private void AddToDbAndUpdateReferential<T>(T value, Action<T> addToReferential)
            where T : class, new()
        {
            //Lock write on calling
            if (value == null)
            {
                return;
            }

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
            {
                Mapper<T>.InsertOne(cnx, value);
            }

            addToReferential(value);
        }
        private void RemoveFromDbAndUpdateReferential<T>(T value, Action<T> removeFromReferential)
            where T : class, new()
        {
            //Lock write on calling
            if (value == null)
            {
                return;
            }

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
            {
                Mapper<T>.DeleteOne(cnx, value);
            }

            removeFromReferential(value);
        }

        private void LoadReferentials()
        {
            //Lock Write on calling
            using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
            {
                _allOptions.Clear();
                _rarities.Clear();
                _blocks.Clear();
                _editions.Clear();
                _languages.Clear();
                _alternativeNameLanguages.Clear();
                _cards.Clear();
                _cardsbyId.Clear();
                _cardFaces.Clear();
                _cardsWithoutSpecialCharacters.Clear();
                _cardEditions.Clear();
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

                foreach (CardFace cardFace in Mapper<CardFace>.LoadAll(cnx))
                {
                    InsertInReferential(cardFace);
                }

                foreach (Translate translate in Mapper<Translate>.LoadAll(cnx))
                {
                    InsertInReferential(translate);
                }

                foreach (CardEdition cardEdition in Mapper<CardEdition>.LoadAll(cnx))
                {
                    InsertInReferential(cardEdition);
                }

                foreach (ExternalIds externalId in Mapper<ExternalIds>.LoadAll(cnx))
                {
                    InsertInReferential(externalId);
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

            _pictureDatabase.LoadAllTreePicture();

            _referentialLoaded = true;
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
            _cards.Add(card.Name, card);
            _cardsbyId.Add(card.Id, card);
            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(CardFace cardFace)
        {
            if (_cardsbyId.GetOrDefault(cardFace.IdCard) is not Card card)
            {
                throw new ApplicationDbException($"Can't find card with id {cardFace.IdCard}");
            }
            _cardFaces.Add(cardFace.Id, cardFace);

            card.AddCardFace(cardFace);

            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(ICardEdition cardEdition)
        {
            _cardEditions.Add(cardEdition.IdScryFall, cardEdition);
            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(IOption option)
        {
            if (!_allOptions.TryGetValue(option.Type, out IList<IOption> options))
            {
                options = new List<IOption>();
                _allOptions.Add(option.Type, options);
            }

            options.Add(option);
        }
        private void InsertInReferential(Translate translate)
        {
            if (_cardsbyId.GetOrDefault(translate.IdCard) is not Card card)
            {
                throw new ApplicationDbException($"Can't find card with id {translate.IdCard}");
            }

            card.AddTranslate(translate);

            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(ExternalIds externalId)
        {
            if (_cardEditions.GetOrDefault(externalId.IdScryFall) is not CardEdition cardEdition)
            {
                throw new ApplicationDbException($"Can't find CardEdition with id {externalId.IdScryFall}");
            }

            cardEdition.AddExternalId(externalId);

            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(Price price)
        {
            if (_cardEditions.GetOrDefault(price.IdScryFall) == null)
            {
                throw new ApplicationDbException($"Can't find CardEdition with id {price.IdScryFall}");
            }

            if (!_prices.TryGetValue(price.IdScryFall, out IList<IPrice> prices))
            {
                prices = new List<IPrice>();
                _prices.Add(price.IdScryFall, prices);
            }

            prices.Add(price);

            _cacheForAllDbInfos = null;
        }
        private void InsertInReferential(ILanguage language)
        {
            _languages.Add(language.Name, language);

            string langName = language.Name.Replace(" ", string.Empty);
            _alternativeNameLanguages[langName] = language;

            if (!string.IsNullOrWhiteSpace(language.AlternativeName))
            {
                foreach (string name in language.AlternativeName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    langName = name.Replace(" ", string.Empty);
                    _alternativeNameLanguages[langName] = language;
                    _alternativeNameLanguages[name] = language;
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
            if (!_preconstructedDeckCards.TryGetValue(preconstructedDeckCardEdition.IdPreconstructedDeck, out IList<IPreconstructedDeckCardEdition> list))
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
            if (!_preconstructedDeckCards.TryGetValue(preconstructedDeckCardEdition.IdPreconstructedDeck, out IList<IPreconstructedDeckCardEdition> list))
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
                ret = ret[0..^5];
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
