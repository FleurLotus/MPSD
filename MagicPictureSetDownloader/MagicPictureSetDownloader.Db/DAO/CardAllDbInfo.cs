namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;

    internal class CardAllDbInfo : ICardAllDbInfo
    {
        private ICollection<ICardInCollectionCount> _statistics;
        public string IdScryFall { get; set; }
        public ICard Card { get; set; }
        public ICollection<ICardFace> CardFaces { get; set; }
        public ICardFace MainCardFace { get; set; }
        public IRarity Rarity { get; set; }
        public IEdition Edition { get; set; }
        public ICollection<IPrice> Prices { get; set; }
        public ICollection<string> VariationIdScryFalls { get; set; }
        public IEnumerable<ICardInCollectionCount> Statistics
        {
            get { return _statistics; }
        }
        internal void SetStatistics(ICollection<ICardInCollectionCount> statistics)
        {
            _statistics = statistics;
        }
    }
}