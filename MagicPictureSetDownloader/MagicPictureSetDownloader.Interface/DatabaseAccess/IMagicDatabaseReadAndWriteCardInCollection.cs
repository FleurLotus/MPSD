namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCardInCollection : IMagicDatabaseReadOnly
    {
        void InsertOrUpdateCardInCollection(int idCollection, string idScryFall, int idLanguage, ICardCount cardCount);
        void MoveCardToOtherCollection(ICardCollection collection, string idScryFall, int idLanguage, ICardCount cardCount, ICardCollection collectionDestination);
    }
}