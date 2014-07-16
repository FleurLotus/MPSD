using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Database;
using Common.Libray;

namespace MagicPictureSetDownloader.Db
{
    public class MagicDatabaseManager
    {
        private readonly object _sync = new object();
        private bool _referentialLoaded;
        private readonly string _connectionString;
        private readonly string _connectionStringForPictureDb;
        private readonly IList<Edition> _editions = new List<Edition>();
        private readonly IDictionary<string, Rarity> _rarities = new Dictionary<string, Rarity>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, Block> _blocks = new Dictionary<int, Block>();
        private readonly IDictionary<int, Picture> _pictures = new Dictionary<int, Picture>();
        private readonly IDictionary<string, Card> _cards = new Dictionary<string, Card>(StringComparer.InvariantCultureIgnoreCase);
        
        public MagicDatabaseManager(string fileName, string pictureFileName)
        {
            _connectionString = "datasource=" + Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            _connectionStringForPictureDb = "datasource=" + Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), pictureFileName);
        }

        public Card GetCard(string name)
        {
            CheckReferentialLoaded();
            return _cards.GetOrDefault(name);
        }
        public int GetRarityId(string rarity)
        {
            CheckReferentialLoaded();
            return _rarities[rarity].Id;
        }
        public void InsertNewPicture(int idGatherer, byte[] data, byte[] foildata = null)
        {
            Picture picture = new Picture { IdGatherer = idGatherer, Image = data, FoilImage = foildata };
            AddToDbAndUpdateReferential(_connectionStringForPictureDb, picture, pic => _pictures.Add(pic.IdGatherer, pic));
        }

        public void InsertNewCard(Card card)
        {
            if (card == null)
                throw new ArgumentNullException("card");
            
            if (string.IsNullOrWhiteSpace(card.Name))
                    throw new ArgumentException("Card.Name");

            if (GetCard(card.Name) != null)
                return;

            AddToDbAndUpdateReferential(_connectionString, card, cd => _cards.Add(cd.Name, cd));
        }

        public Picture GetPicture(int idGatherer)
        {
            Picture picture;

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
        public Edition GetEdition(string sourceName)
        {
            CheckReferentialLoaded();
            Edition edition = _editions.FirstOrDefault(ed => String.Equals(ed.GathererName, sourceName, StringComparison.InvariantCultureIgnoreCase));
            if (edition == null)
            {
                edition = new Edition {Name = sourceName, GathererName = sourceName};
                AddToDbAndUpdateReferential(_connectionString,edition, ed => _editions.Add(ed));
            }

            return edition;
        }

        private void AddToDbAndUpdateReferential<T>(string connectionString, T value, Action<T> addToReferential)
            where T: class, new()
        {
            using (SqlCeConnection cnx = new SqlCeConnection(connectionString))
            {
                cnx.Open();
                Mapper<T>.InsertOne(cnx, value);
            }

            lock (_sync)
                addToReferential(value);
        }
        private Picture LoadImage(int idGatherer)
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

                foreach (Rarity rarity in Mapper<Rarity>.LoadAll(cnx))
                    _rarities.Add(rarity.Code, rarity);

                foreach (Block block in Mapper<Block>.LoadAll(cnx))
                    _blocks.Add(block.Id, block);

                foreach (Edition edition in Mapper<Edition>.LoadAll(cnx))
                {
                    Block block;
                    if (edition.IdBlock.HasValue && _blocks.TryGetValue(edition.IdBlock.Value, out block))
                        edition.BlockName = block.Name;
                    _editions.Add(edition);
                }

                foreach (Card card in Mapper<Card>.LoadAll(cnx))
                    _cards.Add(card.Name, card);

            }
            _referentialLoaded = true;
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
