namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteCollection : IMagicDatabaseReadOnly
    {
        void DeleteCardInCollection(int idCollection, int idGatherer);
        void DeleteAllCardInCollection(string name);
        void DeleteCollection(string name);
        void InsertNewCollection(string name);
        void MoveCollection(string toBeDeletedCollectionName, string toAddCollectionName);
        ICardCollection UpdateCollectionName(ICardCollection collection, string name);
        ICardCollection UpdateCollectionName(string oldName, string name);
     /*   
        
        
        
        void InsertNewOption(TypeOfOption type, string key, string value);
        void InsertNewCardInCollection(int idCollection, int idGatherer, int count, int foilCount = 0);
        ICardInCollectionCount UpdateCardCollectionCount(ICardInCollectionCount cardInCollection, int count, int countFoil = 0);

        
        
        */
    }
}
