namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCardInCollection : IMagicDatabaseReadOnly
    {
        ICardInCollectionCount InsertOrUpdateCardInCollection(int idCollection, int idGatherer, int countToAdd, int foilCountToAdd);
        void MoveCardToOtherCollection(int idCollection, int idGatherer, int countToMove, bool isFoil,int idDestinationCollection);
        void ChangeCardFoil(int idCollection, int idGatherer, int countToChange, bool toFoil);
        void ChangeCardEdition(int idCollection, int idGatherer, int countToChange, bool isFoil, int idEdition);
    }
}
