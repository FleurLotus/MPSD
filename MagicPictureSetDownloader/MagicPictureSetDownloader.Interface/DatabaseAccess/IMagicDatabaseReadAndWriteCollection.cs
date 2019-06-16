namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCollection : IMagicDatabaseReadOnly
    {
        void DeleteAllCardInCollection(string name);
        void DeleteCollection(string name);
        ICardCollection InsertNewCollection(string name);
        void MoveCollection(string toBeDeletedCollectionName, string toAddCollectionName);
        ICardCollection UpdateCollectionName(ICardCollection collection, string name);
        ICardCollection UpdateCollectionName(string oldName, string name);
        void PreconstructedDeckToCollection(IPreconstructedDeck preconstructedDeck, ICardCollection collection);
    }
}
