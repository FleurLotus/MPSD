namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCardInCollection : IMagicDatabaseReadOnly
    {
        void InsertNewCardInCollection(int idCollection, int idGatherer, int count, int foilCount = 0);
        ICardInCollectionCount UpdateCardCollectionCount(ICardInCollectionCount cardInCollection, int count, int countFoil = 0);
    }
}
