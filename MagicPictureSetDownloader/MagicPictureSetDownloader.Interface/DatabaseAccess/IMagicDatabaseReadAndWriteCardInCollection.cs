namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCardInCollection : IMagicDatabaseReadOnly
    {
        ICardInCollectionCount InsertOrUpdateCardInCollection(int idCollection, int idGatherer, int idLanguage, int countToAdd, int foilCountToAdd);
        void MoveCardToOtherCollection(ICardCollection collection, ICard card, IEdition edition, ILanguage language, int countToMove, bool isFoil, ICardCollection collectionDestination);
        void MoveCardToOtherCollection(ICardCollection collection, int idGatherer, int idLanguage, int countToMove, bool isFoil, ICardCollection collectionDestination);
        void ChangeCardEditionFoilLanguage(ICardCollection collection, ICard card, int countToChange, IEdition editionSource, bool isFoilSource, ILanguage languageSource, IEdition editionDestination, bool isFoilDestination, ILanguage languageDestination);
    }
}
