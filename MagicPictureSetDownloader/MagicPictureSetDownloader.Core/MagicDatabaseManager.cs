using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Reflection;
using Common.Database;
using MagicPictureSetDownloader.Core.Db;

namespace MagicPictureSetDownloader.Core
{
    class MagicDatabaseManager
    {
        private readonly object _sync = new object();
        private readonly string _connectionString;
        private readonly IDictionary<string, Rarity> _rarities = new Dictionary<string, Rarity>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IList<Edition> _editions = new List<Edition>();
        private readonly IDictionary<string, Block> _blocks = new Dictionary<string, Block>(StringComparer.InvariantCultureIgnoreCase);
        
        public MagicDatabaseManager(string fileName)
        {
            _connectionString = "datasource=" + Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
        }

        public int GetRarityId(string rarity)
        {
            if (_rarities.Count == 0)
            {
                lock (_sync)
                {
                    if (_rarities.Count == 0)
                    {
                        LoadReferential();
                    }
                }
            }

            return _rarities[rarity].Id;
        }

        private void LoadReferential()
        {
            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                
                foreach (Rarity rarity in Mapper<Rarity>.Load(cnx))
                    _rarities.Add(rarity.Code, rarity);

                foreach (Edition edition in Mapper<Edition>.Load(cnx))
                    _editions.Add(edition);
                
                foreach (Block block in Mapper<Block>.Load(cnx))
                    _blocks.Add(block.Name, block);
            }
        }
    }
}
