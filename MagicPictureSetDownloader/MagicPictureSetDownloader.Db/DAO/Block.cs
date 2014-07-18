using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db.DAO
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Block : IBlock
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
    }
}
