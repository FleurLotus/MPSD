using System.Diagnostics;
using Common.Database.Attribute;

namespace MagicPictureSetDownloader.Core.Db
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    class Rarity
    {
        [DbColumn]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Code { get; set; }
    }
}
