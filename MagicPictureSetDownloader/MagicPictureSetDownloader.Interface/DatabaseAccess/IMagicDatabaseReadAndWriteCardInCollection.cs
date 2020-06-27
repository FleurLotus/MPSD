namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCardInCollection : IMagicDatabaseReadOnly
    {
        void InsertOrUpdateCardInCollection(int idCollection, int idGatherer, int idLanguage, int countToAdd, int foilCountToAdd, int altArtcountToAdd, int foilAltArtCountToAdd);
        void MoveCardToOtherCollection(ICardCollection collection, ICard card, IEdition edition, ILanguage language, int countToMove, bool isFoil, bool isAltArt, ICardCollection collectionDestination);
        void MoveCardToOtherCollection(ICardCollection collection, int idGatherer, int idLanguage, int countToMove, bool isFoil, bool isAltArt, ICardCollection collectionDestination);
        void ChangeCardEditionFoilAltArtLanguage(ICardCollection collection, ICard card, int countToChange, IEdition editionSource, bool isFoilSource, bool isAltArtSource, ILanguage languageSource, IEdition editionDestination, bool isFoilDestination, bool isAltArtDestination, ILanguage languageDestination);
    }
}
