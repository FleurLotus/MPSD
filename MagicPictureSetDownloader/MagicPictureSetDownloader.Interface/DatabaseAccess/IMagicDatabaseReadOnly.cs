namespace MagicPictureSetDownloader.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IMagicDatabaseReadOnly
    {
        ICollection<ICardAllDbInfo> GetAllInfos(int onlyInCollectionId = -1);
        ICollection<IEdition> AllEditions();
        IList<IOption> GetOptions(TypeOfOption type);
        ICard GetCard(string name, string partName);
        Tuple<ICard, IEdition> GetCardxEdition(int idGatherer);
        IEdition GetEditionFromCode(string code);
        int GetGathererId(ICard card, IEdition edition);
        IPicture GetPicture(int idGatherer);
        ITreePicture GetTreePicture(string key);
        IPicture GetDefaultPicture();
        IEdition GetEdition(string sourceName);
        IEdition GetEdition(int idGatherer);
        IOption GetOption(TypeOfOption type, string key);
        ICardCollection GetCollection(int collectionId);
        ICardCollection GetCollection(string name);
        ICollection<ICardCollection> GetAllCollections();
        ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection);
        ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, int idGatherer);
        ICollection<ICardInCollectionCount> GetCardCollectionStatistics(ICard card);
        string[] GetMissingPictureUrls();
    }
}
