namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCardInCollection : IMagicDatabaseReadOnly
    {
        ICardInCollectionCount InsertOrUpdateCardInCollection(int idCollection, int idGatherer, int countToAdd, int foilCountToAdd);
        void MoveCardToOtherCollection(ICardCollection collection, ICard card, IEdition edition,  int countToMove, bool isFoil, ICardCollection collectionDestination);
        void ChangeCardEditionFoil(ICardCollection collection, ICard card, int countToChange, IEdition editionSource, bool isFoilSource, IEdition editionDestination, bool isFoilDestination);
    }
}
