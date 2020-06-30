namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCardInCollection : IMagicDatabaseReadOnly
    {
        void InsertOrUpdateCardInCollection(int idCollection, int idGatherer, int idLanguage, ICardCount cardCount);
        void MoveCardToOtherCollection(ICardCollection collection, ICard card, IEdition edition, ILanguage language, ICardCount cardCount, ICardCollection collectionDestination);
        void MoveCardToOtherCollection(ICardCollection collection, int idGatherer, int idLanguage, ICardCount cardCount, ICardCollection collectionDestination);
        void ChangeCardEditionFoilAltArtLanguage(ICardCollection collection, ICard card, int countToChange, IEdition editionSource, ICardCountKey cardCountKeySource, ILanguage languageSource, IEdition editionDestination, ICardCountKey cardCountKeyDestination, ILanguage languageDestination);
    }
}
