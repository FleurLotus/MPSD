using System.Diagnostics;
using Common.Database.Attribute;

namespace MagicPictureSetDownloader.Core.Db
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    class Block
    {
        [DbColumn]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
    }
}
