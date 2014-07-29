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
    using MagicPictureSetDownloader.Interface;

    public class MagicDatabase
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

        public MagicDatabase(string fileName, string pictureFileName)
        {
            _connectionString = "datasource=" + Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            _connectionStringForPictureDb = "datasource=" + Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), pictureFileName);
        }

        //Unitary Get 
        public ICard GetCard(string name)
        {
            CheckReferentialLoaded();
            return _cards.GetOrDefault(name);
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
            ITreePicture treepicture;

            if (!_treePictures.TryGetValue(key, out treepicture))
            {
                lock (_sync)
                {
                    if (!_treePictures.TryGetValue(key, out treepicture))
                    {
                        treepicture = LoadTreeImage(key);
                        if (treepicture != null)
                            _treePictures.Add(treepicture.Name, treepicture);
                    }
                }
            }

            return treepicture;
        }
        public IEdition GetEdition(string sourceName)
        {
            CheckReferentialLoaded();
            IEdition edition = _editions.FirstOrDefault(ed => String.Equals(ed.GathererName, sourceName, StringComparison.InvariantCultureIgnoreCase));
            if (edition == null)
            {
                Edition realEdition = new Edition { Name = sourceName, GathererName = sourceName };
                AddToDbAndUpdateReferential(_connectionString, realEdition, InsertInReferential);
                edition = realEdition;
            }

            return edition;
        }

        //Ensembly Get 
        public ICollection<ICardAllDbInfo> GetAllInfos()
        {

            CheckReferentialLoaded();
            List<ICardAllDbInfo> ret = _cardEditions.Values.Select(
                    cardEdition => (ICardAllDbInfo) (new CardAllDbInfo
                         {
                             Card = _cards.Values.FirstOrDefault(c => c.Id == cardEdition.IdCard),
                             Edition = _editions.FirstOrDefault(e => e.Id == cardEdition.IdEdition),
                             Rarity = _rarities.Values.FirstOrDefault(r => r.Id == cardEdition.IdRarity),
                             IdGatherer = cardEdition.IdGatherer
                         })).ToList();

            return ret.AsReadOnly();
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
            if (GetPicture(idGatherer) != null)
                return;

            Picture picture = new Picture {IdGatherer = idGatherer, Image = data};
            AddToDbAndUpdateReferential(_connectionStringForPictureDb, picture, InsertInReferential);
        }
        public void InsertNewTreePicture(string name, byte[] data)
        {
            if (GetTreePicture(name) != null)
                return;

            TreePicture treepicture = new TreePicture { Name = name, Image = data };
            AddToDbAndUpdateReferential(_connectionStringForPictureDb, treepicture, InsertInReferential);
        }
        public void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if (GetCard(name) != null)
                return;

            Card card = new Card
            {
                Name = name,
                Text = text,
                Power = power,
                Toughness = toughness,
                CastingCost = castingcost,
                Loyalty = loyalty,
                Type = type
            };

            AddToDbAndUpdateReferential(_connectionString, card, InsertInReferential);
        }
        public void InsertNewCardEdition(int idGatherer, int idEdition, string name, string rarity)
        {
            int idRarity = GetRarityId(rarity);
            int idCard = GetCard(name).Id;
            
            if (idGatherer <= 0 || idEdition <= 0)
                throw new ApplicationDbException("Data are not filled correctedly");

            if (GetCardEdition(idGatherer) != null)
                return;

            CardEdition cardEdition = new CardEdition
            {
                IdCard = idCard,
                IdGatherer = idGatherer,
                IdEdition = idEdition,
                IdRarity = idRarity
            };

            AddToDbAndUpdateReferential(_connectionString, cardEdition, InsertInReferential);
        }

        private void AddToDbAndUpdateReferential<T>(string connectionString, T value, Action<T> addToReferential)
            where T : class, new()
        {
            using (SqlCeConnection cnx = new SqlCeConnection(connectionString))
            {
                cnx.Open();
                Mapper<T>.InsertOne(cnx, value);
            }

            lock (_sync)
                addToReferential(value);
        }
        private ITreePicture LoadTreeImage(string name)
        {
            TreePicture treePicture = new TreePicture { Name = name };

            using (SqlCeConnection cnx = new SqlCeConnection(_connectionStringForPictureDb))
            {
                cnx.Open();
                return Mapper<TreePicture>.Load(cnx, treePicture);
            }
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
                _rarities.Clear();
                _blocks.Clear();
                _editions.Clear();
                _cards.Clear();
                _cardEditions.Clear();

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
            _cards.Add(card.Name, card);
        }
        private void InsertInReferential(ICardEdition cardEdition)
        {
            _cardEditions.Add(cardEdition.IdGatherer, cardEdition);
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