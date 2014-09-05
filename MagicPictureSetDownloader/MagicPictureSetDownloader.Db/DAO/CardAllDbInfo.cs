namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;

    internal class CardAllDbInfo : ICardAllDbInfo
    {
        private readonly List<ICardInCollectionCount> _inCollection;
        
        public CardAllDbInfo()
        {
            _inCollection = new List<ICardInCollectionCount>();
        }
        
        public int IdGatherer { get; set; }
        public ICard Card { get; set; }
        public IRarity Rarity { get; set; }
        public IEdition Edition { get; set; }
        //For Multipart card
        public int IdGathererPart2 { get; set; }
        public ICard CardPart2 { get; set; }
        public ICollection<ICardInCollectionCount> InCollection
        {
            get { return _inCollection.AsReadOnly(); }
        }

        public void Add(ICardInCollectionCount cardInCollectionCount)
        {
            _inCollection.Add(cardInCollectionCount);
        }

    }
}
