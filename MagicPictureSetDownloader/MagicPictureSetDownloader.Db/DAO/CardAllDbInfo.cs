namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;

    internal class CardAllDbInfo : ICardAllDbInfo
    {
        private ICollection<ICardInCollectionCount> _statistics;
        public int IdGatherer { get; set; }
        public ICard Card { get; set; }
        public IRarity Rarity { get; set; }
        public IEdition Edition { get; set; }
        public ICollection<IPrice> Prices { get; set; }
        public ICollection<int> VariationIdGatherers { get; set; }
        //For Multipart card
        public int IdGathererPart2 { get; set; }
        public ICard CardPart2 { get; set; }
        public ICollection<int> VariationIdGatherers2 { get; set; }
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