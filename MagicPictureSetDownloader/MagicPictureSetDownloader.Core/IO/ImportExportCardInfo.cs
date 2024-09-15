 namespace MagicPictureSetDownloader.Core.IO
{
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    internal class ImportExportCardInfo : IImportExportCardCount
    {
        private readonly CardCount _cardCount;

        internal ImportExportCardInfo(string idScryFall, ICardCount cardCount, int idLanguage)
        {
            IdScryFall = idScryFall;
            IdLanguage = idLanguage;
            _cardCount = new CardCount(cardCount);
        }

        public string IdScryFall { get; }
        public int IdLanguage { get; }

        public int Number { get { return GetCount(CardCountKeys.Standard); } }
        public int FoilNumber { get { return GetCount(CardCountKeys.Foil); } }
        public int AltArtNumber { get { return GetCount(CardCountKeys.AltArt); } }
        public int FoilAltArtNumber { get { return GetCount(CardCountKeys.FoilAltArt); } }

        internal void Add(ICardCountKey key, int number)
        {
            _cardCount.Add(key, number);
        }

        public int GetCount(ICardCountKey key)
        {
            return _cardCount.GetCount(key);
        }
        public ICardCount GetCardCount()
        {
            return new CardCount(_cardCount);
        }
    }
}
