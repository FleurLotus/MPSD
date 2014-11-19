namespace MagicPictureSetDownloader.ViewModel.Main
{
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class StatisticViewModel
    {
        public StatisticViewModel(ICardInCollectionCount cardInCollectionCount)
        {
            IMagicDatabaseReadOnly magicDatabase = MagicDatabaseManager.ReadOnly;
            FoilNumber = cardInCollectionCount.FoilNumber;
            Number = cardInCollectionCount.Number;
            Collection = magicDatabase.GetCollection(cardInCollectionCount.IdCollection).Name;
            Edition = magicDatabase.GetEdition(cardInCollectionCount.IdGatherer).Name;
            Language = magicDatabase.GetLanguage(cardInCollectionCount.IdLanguage).Name;
        }

        public int FoilNumber { get; private set; }
        public int Number { get; private set; }
        public string Language { get; private set; }
        public string Edition { get; private set; }
        public string Collection { get; private set; }
    }
}