using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    public class Block
    {
        [DbColumn, DbKeyColumn]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
    }
}
