using System.Diagnostics;
using Common.Database;

namespace MagicPictureSetDownloader.Db
{
    [DebuggerDisplay("{Name}")]
    [DbTable]
    public class Card
    {
        [DbColumn, DbKeyColumn]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Text { get; set; }
        [DbColumn]
        public bool IsLand { get; set; }
        [DbColumn]
        public bool IsCreature { get; set; }
        [DbColumn]
        public bool IsEnchantement { get; set; }
        [DbColumn]
        public bool IsAura { get; set; }
        [DbColumn]
        public bool IsSorcery { get; set; }
        [DbColumn]
        public bool IsInstant { get; set; }
        [DbColumn]
        public bool IsPlanewalker { get; set; }
        [DbColumn]
        public bool IsLegendary{ get; set; }
        [DbColumn]
        public string Power { get; set; }
        [DbColumn]
        public string Toughness { get; set; }
        [DbColumn]
        public string CastingCost { get; set; }
        [DbColumn]
        public int? Loyalty { get; set; }

    }
}
