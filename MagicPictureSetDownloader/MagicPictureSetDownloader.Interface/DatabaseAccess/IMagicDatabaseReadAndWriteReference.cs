namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteReference : IMagicDatabaseReadOnly
    {
        void InsertNewPicture(int idGatherer, byte[] data);
        void InsertNewTreePicture(string key, byte[] data);
        void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type, string partName, string otherPartName);
        void InsertNewCardEdition(int idGatherer, int idEdition, string name, string partName, string rarity, string url);
        void EditionCompleted(int editionId);
    }
}
