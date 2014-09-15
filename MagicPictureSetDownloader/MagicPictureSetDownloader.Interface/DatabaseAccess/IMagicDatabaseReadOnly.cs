namespace MagicPictureSetDownloader.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IMagicDatabaseReadOnly
    {
        ICollection<ICardAllDbInfo> GetAllInfos(bool withCollectionInfo, int onlyInCollectionId);
        IList<IOption> GetOptions(TypeOfOption type);
        ICard GetCard(string name, string partName);
        Tuple<ICard, IEdition> GetCardxEdition(int idGatherer);
        IEdition GetEditionFromCode(string code);
        int GetGathererId(ICard card, IEdition edition);
        IPicture GetPicture(int idGatherer);
        ITreePicture GetTreePicture(string key);
        IPicture GetDefaultPicture();
        IEdition GetEdition(string sourceName);
        IOption GetOption(TypeOfOption type, string key);
        ICardCollection GetCollection(string name);
        ICollection<ICardCollection> GetAllCollections();
        IList<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection);
        string[] GetMissingPictureUrls();
    }
}
