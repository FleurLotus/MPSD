using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Database;

namespace MagicPictureSetDownloader.Db
{
    public class MagicDatabaseManager
    {
        private readonly object _sync = new object();
        private bool _referentialLoaded;
        private readonly string _connectionString;
        private readonly IList<Edition> _editions = new List<Edition>();
        private readonly IDictionary<string, Rarity> _rarities = new Dictionary<string, Rarity>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, Block> _blocks = new Dictionary<int, Block>();
        private readonly IDictionary<int, Picture> _pictures = new Dictionary<int, Picture>();
        private readonly IDictionary<string, Card> _cards = new Dictionary<string, Card>(StringComparer.InvariantCultureIgnoreCase);
        
        public MagicDatabaseManager(string fileName)
        {
            _connectionString = "datasource=" + Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
        }

        public int GetRarityId(string rarity)
        {
            CheckReferentialLoaded();
            return _rarities[rarity].Id;
        }
        public void InsertNewPicture(byte[] data)
        {
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                Mapper<Picture>.InsertOne(cnx, new Picture {Data = data});
            }
        }
        public Picture GetPicture(int idPicture)
        {
            Picture picture;
            
            if (!_pictures.TryGetValue(idPicture, out picture))
            {
                lock (_sync)
                {
                    if (!_pictures.TryGetValue(idPicture, out picture))
                    {
                        picture = LoadImage(idPicture);
                        _pictures.Add(picture.Id, picture);
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
                AddToDbAndUpdateReferential(edition, ed=> _editions.Add(edition));
            }

            return edition;
        }

        private void AddToDbAndUpdateReferential<T>(T value, Action<T> addToReferential)
            where T: class, new()
        {
            addToReferential(value);
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                Mapper<T>.InsertOne(cnx, value);
            }
        }
        private Picture LoadImage(int idPicture)
        {
            Picture picture = new Picture{Id = idPicture};

            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                Mapper<Picture>.Load(cnx, picture);

            }
            return picture;
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
