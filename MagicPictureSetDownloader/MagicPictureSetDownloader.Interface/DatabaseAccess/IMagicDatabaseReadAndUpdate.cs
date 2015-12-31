namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IMagicDatabaseReadAndUpdate : IMagicDatabaseReadAndWriteReference
    {
        void UpdateEdition(IEdition edition, string sourceName, string name, bool hasFoil, string code, int? idBlock, int? blockPosition, int? cardNumber, DateTime? releaseDate);
        void UpdateBlock(IBlock block, string blockName);
        void UpdateLanguage(ILanguage language, string languageName, string alternativeName);
        void UpdateTranslate(ICard card, ILanguage language, string translation);
    }
}
