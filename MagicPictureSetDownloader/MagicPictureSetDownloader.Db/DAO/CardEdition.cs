using Common.Database;

namespace MagicPictureSetDownloader.Db.DAO
{
    [DbTable]
    internal class CardEdition : ICardEdition
    {
        [DbColumn]
        public int IdEdition { get; set; }
        [DbColumn]
        public int IdCard { get; set; }
        [DbColumn]
        public int IdRarity { get; set; }
        [DbColumn]
        public int IdGatherer { get; set; }
    }
}
