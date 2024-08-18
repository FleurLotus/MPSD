namespace MagicPictureSetDownloader.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IMagicDatabaseReadAndWriteReference : IMagicDatabaseReadOnly
    {
        void InsertNewPicture(string idScryFall, byte[] data);
        void InsertNewTreePicture(string key, byte[] data);
        void InsertNewEdition(string sourceName);
        void InsertNewEdition(string sourceName, string name, bool hasFoil, string code, int? idBlock, int? blockPosition, int? cardNumber, DateTime? releaseDate, byte[] icon);
        void InsertNewCard(string name);
        void InsertNewCardFace(int idCard, string idScryFall, string name, string text, string power, string toughness, string castingcost, string loyalty, string defense, string type, string url, IDictionary<string, string> languages);
        void InsertNewCardEdition(string idScryFall, int idEdition, string name, string rarity);
        void InsertNewCardEditionVariation(string idScryFall, string otherIdScryFall, string url);
        void InsertNewBlock(string blockName);
        void InsertNewLanguage(string languageName, string alternativeName);
        void InsertNewRuling(string idScryFall, DateTime addDate, string text);
        void InsertNewPrice(string idScryFall, DateTime addDate, string source, bool foil, int value);
        void InsertNewPreconstructedDeck(int idEdition, string preconstructedDeckName, string url);
        void InsertOrUpdatePreconstructedDeckCardEdition(int idPreconstructedDeck, string idScryFall, int count);
        void EditionCompleted(int editionId);
    }
}
