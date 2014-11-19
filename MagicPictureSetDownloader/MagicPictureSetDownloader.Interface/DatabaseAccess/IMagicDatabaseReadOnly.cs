namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface IMagicDatabaseReadOnly
    {
        ICollection<ICardAllDbInfo> GetAllInfos(int onlyInCollectionId = -1);
        ICollection<IEdition> AllEditions();
        IList<IOption> GetOptions(TypeOfOption type);
        ICard GetCard(string name, string partName);
        IEdition GetEditionFromCode(string code);
        int GetIdGatherer(ICard card, IEdition edition);
        IPicture GetPicture(int idGatherer);
        ITreePicture GetTreePicture(string key);
        IPicture GetDefaultPicture();
        IEdition GetEdition(string sourceName);
        IEdition GetEdition(int idGatherer);
        ILanguage GetLanguage(int idLanguage);
        ICard GetCard(int idGatherer);
        IList<ILanguage> GetLanguages(int idGatherer);
        IOption GetOption(TypeOfOption type, string key);
        ICardCollection GetCollection(int collectionId);
        ICardCollection GetCollection(string name);
        ICollection<ICardCollection> GetAllCollections();
        ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection);
        ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection, int idGatherer);
        ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, int idGatherer, int idLanguage);
        ICollection<ICardInCollectionCount> GetCardCollectionStatistics(ICard card);
        ITranslate GetTranslate(ICard card, int idLanguage);
        string[] GetMissingPictureUrls();
    }
}
