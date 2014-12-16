namespace MagicPictureSetDownloader.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IMagicDatabaseReadAndWriteReference : IMagicDatabaseReadOnly
    {
        void InsertNewPicture(int idGatherer, byte[] data);
        void InsertNewTreePicture(string key, byte[] data);
        void CreateNewEdition(string sourceName);
        void CreateNewEdition(string sourceName, string name, bool hasFoil, string code, int? idBlock, int? blockPosition, int? cardNumber, DateTime? releaseDate);
        void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type, string partName, string otherPartName, IDictionary<string, string> languages);
        void InsertNewCardEdition(int idGatherer, int idEdition, string name, string partName, string rarity, string url);
        void EditionCompleted(int editionId);
    }
}
