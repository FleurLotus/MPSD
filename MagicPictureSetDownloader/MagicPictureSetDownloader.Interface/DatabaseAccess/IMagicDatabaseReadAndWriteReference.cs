namespace MagicPictureSetDownloader.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IMagicDatabaseReadAndWriteReference : IMagicDatabaseReadOnly
    {
        int InsertNewCardEditionWithFakeGathererId(int idEdition, int idCard, int idRarity, string url);
        void InsertNewPicture(int idGatherer, byte[] data);
        void InsertNewTreePicture(string key, byte[] data);
        void InsertNewEdition(string sourceName);
        void InsertNewEdition(string sourceName, string name, bool hasFoil, string code, int? idBlock, int? blockPosition, int? cardNumber, DateTime? releaseDate, byte[] icon);
        void InsertNewCard(string name, string text, string power, string toughness, string castingcost, string loyalty, string defense, string type, string partName, string otherPartName, IDictionary<string, string> languages);
        void InsertNewCardEdition(int idGatherer, int idEdition, string name, string partName, string rarity, string url);
        void InsertNewCardEditionVariation(int idGatherer, int otherGathererId, string url);
        void InsertNewBlock(string blockName);
        void InsertNewLanguage(string languageName, string alternativeName);
        void InsertNewRuling(int idGatherer, DateTime addDate, string text);
        void InsertNewPrice(int idGatherer, DateTime addDate, string source, bool foil, int value);
        void InsertNewPreconstructedDeck(int idEdition, string preconstructedDeckName, string url);
        void InsertOrUpdatePreconstructedDeckCardEdition(int idPreconstructedDeck, int idGatherer, int count);
        void EditionCompleted(int editionId);
    }
}
