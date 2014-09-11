namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlServerCe;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common.Database;
    using Common.Libray;
    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.DbGenerator;
    using MagicPictureSetDownloader.Interface;

    public partial class MagicDatabase
    {
        private readonly object _sync = new object();
        private bool _referentialLoaded;
        private readonly string _connectionString;
        private readonly string _connectionStringForPictureDb;

        private readonly IList<IEdition> _editions = new List<IEdition>();
        private readonly IDictionary<string, IRarity> _rarities = new Dictionary<string, IRarity>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, IBlock> _blocks = new Dictionary<int, IBlock>();
        private readonly IDictionary<int, IPicture> _pictures = new Dictionary<int, IPicture>();
        private readonly IDictionary<string, ITreePicture> _treePictures = new Dictionary<string, ITreePicture>();
        private readonly IDictionary<string, ICard> _cards = new Dictionary<string, ICard>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, ICardEdition> _cardEditions = new Dictionary<int, ICardEdition>();
        private readonly IDictionary<TypeOfOption, IList<IOption>> _allOptions = new Dictionary<TypeOfOption, IList<IOption>>();

        public MagicDatabase(string fileName, string pictureFileName)
        {
            string mainDbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            _connectionString = "datasource=" + mainDbPath;
            if (!File.Exists(mainDbPath))
                DatabaseGenerator.GenerateMagicData(_connectionString);

            string pictureDbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), pictureFileName);
            _connectionStringForPictureDb = "datasource=" + pictureDbPath;
            if (!File.Exists(pictureDbPath))
                DatabaseGenerator.GenerateMagicPicture(_connectionStringForPictureDb);
        }

        //Unitary Get 
        public ICard GetCard(string name, string partName)
        {
            CheckReferentialLoaded();
            if (partName == null || partName == name)
                return _cards.GetOrDefault(name);

            return _cards.GetOrDefault(name + partName);
        }

        private ICardEdition GetCardEdition(int idGatherer)
        {
            CheckReferentialLoaded();

            return _cardEditions.GetOrDefault(idGatherer);
        }
        public int GetRarityId(string rarity)
        {
            CheckReferentialLoaded();
            return _rarities[rarity].Id;
        }
        public IPicture GetDefaultPicture()
        {
            return GetPicture(0);
        }
        public IPicture GetPicture(int idGatherer)
        {
            IPicture picture;

            if (!_pictures.TryGetValue(idGatherer, out picture))
            {
                lock (_sync)
                {
                    if (!_pictures.TryGetValue(idGatherer, out picture))
                    {
                        picture = LoadImage(idGatherer);
                        if (picture != null)
                            _pictures.Add(picture.IdGatherer, picture);
                    }
                }
            }

            return picture;
        }
        public ITreePicture GetTreePicture(string key)
        {
            CheckReferentialLoaded();

            return _treePictures.GetOrDefault(key);
        }
        public IEdition GetEdition(string sourceName)
        {
            CheckReferentialLoaded();
            IEdition edition = _editions.FirstOrDefault(ed => String.Equals(ed.GathererName, sourceName, StringComparison.InvariantCultureIgnoreCase));
            if (edition == null)
            {
                Edition realEdition = new Edition { Name = sourceName, GathererName = sourceName, Completed = false };
                AddToDbAndUpdateReferential(_connectionString, realEdition, InsertInReferential);
                edition = realEdition;
            }

            return edition;
        }
        public IOption GetOption(TypeOfOption type, string key)
        {
            IList<IOption> options = GetOptions(type);
            return options == null ? null : options.FirstOrDefault(o => o.Key == key);
        }

        //Ensembly Get 
        public ICollection<ICardAllDbInfo> GetAllInfos(bool withCollectionInfo, int onlyInCollectionId)
        {
            CheckReferentialLoaded();
            List<ICardAllDbInfo> ret = new List<ICardAllDbInfo>();
            foreach (ICardEdition cardEdition in _cardEditions.Values)
            {
                CardAllDbInfo cardAllDbInfo = new CardAllDbInfo();
                if (withCollectionInfo)
                {
                    bool inWantedCollection = false;
                    foreach (ICardInCollectionCount cardInCollectionCount in _allCardInCollectionCount.Values.SelectMany(a => a))
                    {
                        inWantedCollection = inWantedCollection || cardInCollectionCount.IdCollection == onlyInCollectionId;
                        cardAllDbInfo.Add(cardInCollectionCount);
                    }
                    if (!inWantedCollection)
                        continue;
                }

                ICardEdition edition = cardEdition;
                ICard card = _cards.Values.FirstOrDefault(c => c.Id == edition.IdCard);
                cardAllDbInfo.Card = card;
                cardAllDbInfo.Edition = _editions.FirstOrDefault(e => e.Id == edition.IdEdition);
                cardAllDbInfo.Rarity = _rarities.Values.FirstOrDefault(r => r.Id == edition.IdRarity);
                cardAllDbInfo.IdGatherer = cardEdition.IdGatherer;
                cardAllDbInfo.IdGathererPart2 = -1;

                //For Multipart card
                if (card.IsMultiPart)
                {
                    if (card.IsReverseSide)
                        continue;

                    ICard cardPart2 = card.IsSplitted ? GetCard(card.Name, card.OtherPartName) : GetCard(card.OtherPartName, null);
                    cardAllDbInfo.CardPart2 = cardPart2;

                    ICardEdition cardEdition2 = _cardEditions.Values.FirstOrDefault(ce => ce.IdEdition == edition.IdEdition && ce.IdCard == cardPart2.Id);
                    //Verso of Reserse Card
                    if (cardEdition2 != null)
                    {
                        if (cardEdition2.IdGatherer != cardEdition.IdGatherer)
                            cardAllDbInfo.IdGathererPart2 = cardEdition2.IdGatherer;
                    }
                }

                ret.Add(cardAllDbInfo);
            }

            return ret.AsReadOnly();
        }
        public IList<IOption> GetOptions(TypeOfOption type)
        {
            CheckReferentialLoaded();
            IList<IOption> options;
            if (!_allOptions.TryGetValue(type, out options))
                return null;
            return new List<IOption>(options).AsReadOnly();
        }

        public ICollection<IEdition> AllEditions()
        {
            CheckReferentialLoaded();
            return new List<IEdition>(_editions).AsReadOnly();
        }
        public ICollection<IRarity> AllRarities()
        {
            CheckReferentialLoaded();
            return new List<IRarity>(_rarities.Values).AsReadOnly();
        }
        public ICollection<IBlock> AllBlocks()
        {
            CheckReferentialLoaded();
            return new List<IBlock>(_blocks.Values).AsReadOnly();
        }
        public ICollection<ICard> AllCards()
        {
            CheckReferentialLoaded();
            return new List<ICard>(_cards.Values).AsReadOnly();
        }
        public ICollection<ICardEdition> AllCardEditions()
        {
            CheckReferentialLoaded();
            return new List<ICardEdition>(_cardEditions.Values).AsReadOnly();
        }

        //Insert one new 
        public void InsertNewPicture(int idGatherer, byte[] data)
        {
            if (GetPicture(idGatherer) != null || data == null || data.Length == 0)
                return;

            Picture picture = new Picture {IdGatherer = idGatherer, Image = data};
            AddToDbAndUpdateReferential(_connectionStringForPictureDb, picture, InsertInReferential);
        }
        public void InsertNewTreePicture(string name, byte[] data)
        {
            if (GetTreePicture(name) != null || data == null || data.Length == 0)
                return;

            TreePicture treepicture = new TreePicture { Name = name, Image = data };
            AddToDbAndUpdateReferential(_connectionStringForPictureDb, treepicture, InsertInReferential);
        }
        public void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type, string partName, string otherPartName)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if (GetCard(name, partName) != null)
                return;

            Card card = new Card
            {
                PartName = partName??name,
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
        }
        public void InsertNewCardEdition(int idGatherer, int idEdition, string name, string partName, string rarity, string url)
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
        public void InsertNewOption(TypeOfOption type, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                throw new ApplicationDbException("Data are not filled correctedly");
            
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

        public void EditionCompleted(int editionId)
        {
            Edition newEdition = _editions.FirstOrDefault(e=>e.Id == editionId) as Edition;
            if (newEdition == null || newEdition.Completed)
                return;

            lock (_sync)
            {
                newEdition.Completed = true;

                using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
                {
                    cnx.Open();
                    Mapper<Edition>.UpdateOne(cnx, newEdition);
                }
            }

        }
        
        public string[] GetMissingPictureUrls()
        {
            IList<int> idGatherers;
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionStringForPictureDb))
            {
                cnx.Open();
                idGatherers = Mapper<PictureKey>.LoadAll(cnx).Select(pk => pk.IdGatherer).ToList();
            }

            return AllCardEditions().Where(ce => !string.IsNullOrWhiteSpace(ce.Url) && !idGatherers.Contains(ce.IdGatherer))
                                    .Select(ce => ce.Url).ToArray();
        }

        private void AddToDbAndUpdateReferential<T>(string connectionString, T value, Action<T> addToReferential)
            where T : class, new()
        {
            if (value == null)
                return;

            using (SqlCeConnection cnx = new SqlCeConnection(connectionString))
            {
                cnx.Open();
                Mapper<T>.InsertOne(cnx, value);
            }

            lock (_sync)
                addToReferential(value);
        }
        private void RemoveFromDbAndUpdateReferential<T>(string connectionString, T value, Action<T> removeFromReferential)
            where T : class, new()
        {
            if (value == null)
                return;

            using (SqlCeConnection cnx = new SqlCeConnection(connectionString))
            {
                cnx.Open();
                Mapper<T>.DeleteOne(cnx, value);
            }

            lock (_sync)
                removeFromReferential(value);
        }

        private IPicture LoadImage(int idGatherer)
        {
            Picture picture = new Picture { IdGatherer = idGatherer };
            
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionStringForPictureDb))
            {
                cnx.Open();
                return Mapper<Picture>.Load(cnx, picture);
            }
        }
        private void LoadReferentials()
        {
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                _allOptions.Clear();
                _rarities.Clear();
                _blocks.Clear();
                _editions.Clear();
                _cards.Clear();
                _cardEditions.Clear();
                _collections.Clear();
                _allCardInCollectionCount.Clear();

                foreach (Option option in Mapper<Option>.LoadAll(cnx))
                    InsertInReferential(option);

                foreach (Rarity rarity in Mapper<Rarity>.LoadAll(cnx))
                    InsertInReferential(rarity);

                foreach (Block block in Mapper<Block>.LoadAll(cnx))
                    _blocks.Add(block.Id, block);

                foreach (Edition edition in Mapper<Edition>.LoadAll(cnx))
                {
                    if (edition.IdBlock.HasValue)
                        edition.BlockName = _blocks.GetOrDefault(edition.IdBlock.Value).Name;
                    InsertInReferential(edition);
                }

                foreach (Card card in Mapper<Card>.LoadAll(cnx))
                    InsertInReferential(card);

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

        private void RemoveFromReferential(IOption option)
        {
            IList<IOption> options = _allOptions[option.Type];
            options.Remove(option);
            if (options.Count == 0)
                _allOptions.Remove(option.Type);
        }

        private void CheckReferentialLoaded()
        {
            if (!_referentialLoaded)
            {
                lock (_sync)
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
