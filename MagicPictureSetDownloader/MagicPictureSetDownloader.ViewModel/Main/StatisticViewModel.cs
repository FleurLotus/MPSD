namespace MagicPictureSetDownloader.ViewModel.Main
{
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class StatisticViewModel
    {
        public StatisticViewModel(ICardInCollectionCount cardInCollectionCount)
        {
            IMagicDatabaseReadOnly magicDatabase = MagicDatabaseManager.ReadOnly;
            FoilAltArtNumber = cardInCollectionCount.FoilAltArtNumber;
            AltArtNumber = cardInCollectionCount.AltArtNumber;
            FoilNumber = cardInCollectionCount.FoilNumber;
            Number = cardInCollectionCount.Number;
            Collection = magicDatabase.GetCollection(cardInCollectionCount.IdCollection).Name;
            Edition = magicDatabase.GetEditionByIdScryFall(cardInCollectionCount.IdScryFall).Name;
            Language = magicDatabase.GetLanguage(cardInCollectionCount.IdLanguage).Name;
        }

        public int FoilAltArtNumber { get; }
        public int AltArtNumber { get; }
        public int FoilNumber { get; }
        public int Number { get; }
        public string Language { get; }
        public string Edition { get; }
        public string Collection { get; }
    }
}