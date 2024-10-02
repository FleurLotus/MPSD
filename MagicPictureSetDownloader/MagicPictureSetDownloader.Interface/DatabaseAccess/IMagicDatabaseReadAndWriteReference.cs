namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IMagicDatabaseReadAndWriteReference : IMagicDatabaseReadOnly
    {
        void InsertNewPicture(string idScryFall, byte[] data);
        void InsertNewTreePicture(string key, byte[] data, bool isSvg);
        void InsertNewEdition(string sourceName, bool hasFoil, string code, int? idBlock, int? cardNumber, DateTime? releaseDate, byte[] icon);
        void InsertNewCard(string name, string layout);
        void InsertNewTranslate(int idCard, string language, string name);
        void InsertNewCardFace(int idCard, bool isMainFace, string name, string text, string power, string toughness, string castingcost, string loyalty, string defense, string type);
        void InsertNewCardEdition(string idScryFall, string editionCode, string name, string rarity, string url, string url2);
        void InsertNewExternalIds(string idScryFall, CardIdSource cardIdSource, string externalId);
        void InsertNewBlock(string blockName);
        void InsertNewLanguage(string languageName, string alternativeName);
        void InsertNewPrice(string idScryFall, DateTime addDate, string source, bool foil, int value);
        void InsertNewPreconstructedDeck(int? idEdition, string preconstructedDeckName, string url);
        void InsertOrUpdatePreconstructedDeckCardEdition(int idPreconstructedDeck, string idScryFall, int count);
    }
}
