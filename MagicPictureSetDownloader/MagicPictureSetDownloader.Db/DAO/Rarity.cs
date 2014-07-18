using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db.DAO
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Rarity : IRarity
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Code { get; set; }
    }
}
