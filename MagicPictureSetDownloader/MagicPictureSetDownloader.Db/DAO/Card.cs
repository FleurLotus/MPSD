using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db.DAO
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Card : ICard
    {
        [DbColumn, DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Text { get; set; }
        [DbColumn]
        public string Power { get; set; }
        [DbColumn]
        public string Toughness { get; set; }
        [DbColumn]
        public string CastingCost { get; set; }
        [DbColumn]
        public int? Loyalty { get; set; }
        [DbColumn]
        public string Type { get; set; }
    }
}
