namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlServerCe;

    using Common.Database;
    using Common.Libray;
    using Common.Libray.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        private bool _referentialLoaded;
        private readonly string _connectionString;
        private readonly string _connectionStringForPictureDb;

        private readonly IList<IEdition> _editions = new List<IEdition>();
        private readonly IDictionary<string, ILanguage> _languages = new Dictionary<string, ILanguage>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, ILanguage> _alternativeNameLanguages = new Dictionary<string, ILanguage>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, ITranslate> _translates = new Dictionary<int, ITranslate>();
        private readonly IDictionary<string, IRarity> _rarities = new Dictionary<string, IRarity>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, IBlock> _blocks = new Dictionary<int, IBlock>();
        private readonly IDictionary<int, IPicture> _pictures = new Dictionary<int, IPicture>();
        private readonly IDictionary<string, ITreePicture> _treePictures = new Dictionary<string, ITreePicture>();
        private readonly IDictionary<string, ICard> _cards = new Dictionary<string, ICard>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, ICardEdition> _cardEditions = new Dictionary<int, ICardEdition>();
        private readonly IDictionary<TypeOfOption, IList<IOption>> _allOptions = new Dictionary<TypeOfOption, IList<IOption>>();

        //Insert one new
        public void InsertNewEdition(string sourceName)
        {
            using (new WriterLock(_lock))
            {
                IEdition edition = GetEdition(sourceName);
                if (edition == null)
                {
                    Edition realEdition = new Edition { Name = sourceName, GathererName = sourceName, Completed = false, HasFoil = true };
                    AddToDbAndUpdateReferential(_connectionString, realEdition, InsertInReferential);
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
                        realEdition.Block = _blocks.GetOrDefault(realEdition.IdBlock.Value);

                    AddToDbAndUpdateReferential(_connectionString, realEdition, InsertInReferential);
                }
                InsertNewTreePicture(name, icon);
            }
        }
        public void InsertNewPicture(int idGatherer, byte[] data)
        {
            using (new WriterLock(_lock))
            {
                if (GetPicture(idGatherer) != null || data == null || data.Length == 0)
                    return;

                Picture picture = new Picture { IdGatherer = idGatherer, Image = data };
                AddToDbAndUpdateReferential(_connectionStringForPictureDb, picture, InsertInReferential);
            }
        }
        public void InsertNewTreePicture(string name, byte[] data)
        {
            using (new WriterLock(_lock))
            {
                if (GetTreePicture(name) != null || data == null || data.Length == 0)
                    return;

                TreePicture treepicture = new TreePicture { Name = name, Image = data };
                AddToDbAndUpdateReferential(_connectionStringForPictureDb, treepicture, InsertInReferential);
            }
        }
        public void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type, string partName, string otherPartName, IDictionary<string, string> languages)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

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

                    AddToDbAndUpdateReferential(_connectionString, card, InsertInReferential);
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

                if (idGatherer <= 0 || idEdition <= 0)
                    throw new ApplicationDbException("Data are not filled correctedly");

                if (GetCardEdition(idGatherer) != null)
                    return;

                CardEdition cardEdition = new CardEdition
                {
                    IdCard = idCard,
                    IdGatherer = idGatherer,
                    IdEdition = idEdition,
                    IdRarity = idRarity,
                    Url = url
                };

                AddToDbAndUpdateReferential(_connectionString, cardEdition, InsertInReferential);
            }
        }
        public void InsertNewOption(TypeOfOption type, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ApplicationDbException("Data are not filled correctedly");

            using (new WriterLock(_lock))
            {
                IOption option = GetOption(type, key);

                if (option == null)
                {
                    Option newoption = new Option { Type = type, Key = key, Value = value };
                    AddToDbAndUpdateReferential(_connectionString, newoption, InsertInReferential);
                }
                else if (option.Value != value)
                {
                    RemoveFromDbAndUpdateReferential(_connectionString, option as Option, RemoveFromReferential);

                    Option newoption = new Option { Type = type, Key = key, Value = value };
                    AddToDbAndUpdateReferential(_connectionString, newoption, InsertInReferential);
                }
            }
        }
        public void InsertNewBlock(string blockName)
        {
            using (new WriterLock(_lock))
            {
                if (GetBlock(blockName) != null)
                    return;

                Block block = new Block { Name = blockName };
                AddToDbAndUpdateReferential(_connectionString, block, InsertInReferential);
            }
        }
        public void InsertNewLanguage(string languageName, string alternativeName)
        {
            using (new WriterLock(_lock))
            {
                if (GetLanguage(languageName) != null)
                    return;

                Language language = new Language { Name = languageName, AlternativeName = alternativeName };
                AddToDbAndUpdateReferential(_connectionString, language, InsertInReferential);
            }
        }
        private void InsertNewTranslate(ICard card, int idLanguage, string name)
        {
            if (card == null)
                return;

            using (new WriterLock(_lock))
            {
                if (GetTranslate(card, idLanguage) == null)
                {
                    Translate translate = new Translate { IdCard = card.Id, IdLanguage = idLanguage, Name = name };
                    AddToDbAndUpdateReferential(_connectionString, translate, InsertInReferential);
                }
            }
        }

        private void AddToDbAndUpdateReferential<T>(string connectionString, T value, Action<T> addToReferential)
            where T : class, new()
        {
            //Lock write on calling
            if (value == null)
                return;

            using (SqlCeConnection cnx = new SqlCeConnection(connectionString))
            {
                cnx.Open();
                Mapper<T>.InsertOne(cnx, value);
            }

            addToReferential(value);
        }
        private void RemoveFromDbAndUpdateReferential<T>(string connectionString, T value, Action<T> removeFromReferential)
            where T : class, new()
        {
            //Lock write on calling
            if (value == null)
                return;

            using (SqlCeConnection cnx = new SqlCeConnection(connectionString))
            {
                cnx.Open();
                Mapper<T>.DeleteOne(cnx, value);
            }

            removeFromReferential(value);
        }

        private void LoadReferentials()
        {
            //Lock Write on calling
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                _allOptions.Clear();
                _rarities.Clear();
                _blocks.Clear();
                _editions.Clear();
                _languages.Clear();
                _alternativeNameLanguages.Clear();
                _translates.Clear();
                _cards.Clear();
                _cardEditions.Clear();
                _collections.Clear();
                _allCardInCollectionCount.Clear();

                foreach (Option option in Mapper<Option>.LoadAll(cnx))
                    InsertInReferential(option);

                foreach (Rarity rarity in Mapper<Rarity>.LoadAll(cnx))
                    InsertInReferential(rarity);

                foreach (Block block in Mapper<Block>.LoadAll(cnx))
                    InsertInReferential(block);

                foreach (Language language in Mapper<Language>.LoadAll(cnx))
                    InsertInReferential(language);

                foreach (Edition edition in Mapper<Edition>.LoadAll(cnx))
                {
                    if (edition.IdBlock.HasValue)
                        edition.Block = _blocks.GetOrDefault(edition.IdBlock.Value);
                    InsertInReferential(edition);
                }

                foreach (Card card in Mapper<Card>.LoadAll(cnx))
                    InsertInReferential(card);

                foreach (Translate translate in Mapper<Translate>.LoadAll(cnx))
                    InsertInReferential(translate);

                foreach (CardEdition cardEdition in Mapper<CardEdition>.LoadAll(cnx))
                    InsertInReferential(cardEdition);

                foreach (CardCollection cardCollection in Mapper<CardCollection>.LoadAll(cnx))
                    InsertInReferential(cardCollection);

                foreach (CardInCollectionCount cardInCollectionCount in Mapper<CardInCollectionCount>.LoadAll(cnx))
                    InsertInReferential(cardInCollectionCount);

            }
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionStringForPictureDb))
            {
                cnx.Open();
                _treePictures.Clear();

                foreach (TreePicture treePicture in Mapper<TreePicture>.LoadAll(cnx))
                    InsertInReferential(treePicture);
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
        }
        private void InsertInReferential(ICard card)
        {
            if (card.PartName == null || card.Name == card.PartName)
                _cards.Add(card.Name, card);
            else
                _cards.Add(card.Name + card.PartName, card);

        }
        private void InsertInReferential(ICardEdition cardEdition)
        {
            _cardEditions.Add(cardEdition.IdGatherer, cardEdition);
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
        private void InsertInReferential(ITranslate translate)
        {
            int key = TranslateKey(translate.IdCard, translate.IdLanguage);
            _translates.Add(key, translate);
        }
        private void InsertInReferential(ILanguage language)
        {
            _languages.Add(language.Name, language);
            if (!string.IsNullOrWhiteSpace(language.AlternativeName))
            {
                foreach (string name in language.AlternativeName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!_languages.ContainsKey(name) && !_alternativeNameLanguages.ContainsKey(name))
                        _alternativeNameLanguages.Add(name, language);
                }
            }
        }
        private void InsertInReferential(IBlock block)
        {
            _blocks.Add(block.Id, block);
        }

        private void RemoveFromReferential(IOption option)
        {
            IList<IOption> options = _allOptions[option.Type];
            options.Remove(option);
            if (options.Count == 0)
                _allOptions.Remove(option.Type);
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
    }
}