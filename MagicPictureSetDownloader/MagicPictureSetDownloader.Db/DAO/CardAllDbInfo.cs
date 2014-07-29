namespace MagicPictureSetDownloader.Db.DAO
{
    using MagicPictureSetDownloader.Interface;

    internal class CardAllDbInfo : ICardAllDbInfo
    {
        public int IdGatherer { get; set; }
        public ICard Card { get; set; }
        public IRarity Rarity { get; set; }
        public IEdition Edition { get; set; }
    }
}
