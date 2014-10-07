namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;

    internal class CardAllDbInfo : ICardAllDbInfo
    {
        public int IdGatherer { get; set; }
        public ICard Card { get; set; }
        public IRarity Rarity { get; set; }
        public IEdition Edition { get; set; }
        //For Multipart card
        public int IdGathererPart2 { get; set; }
        public ICard CardPart2 { get; set; }
        public ICollection<ICardInCollectionCount> Statistics { get; set; }
    }
}
