﻿namespace MagicPictureSetDownloader.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IMagicDatabaseReadOnly
    {
        ICollection<ICardAllDbInfo> GetAllInfos(int onlyInCollectionId = -1);
        ICollection<IEdition> GetAllEditions();
        ICollection<ILanguage> GetAllLanguages();
        ICollection<ICardCollection> GetAllCollections();
        ICollection<IBlock> GetAllBlocks();
        ICollection<IAudit> GetAllAudits();
        ICollection<IPreconstructedDeck> GetAllPreconstructedDecks();

        IList<IOption> GetOptions(TypeOfOption type);
        IOption GetOption(TypeOfOption type, string key);
        ICard GetCard(string name);
        ICardEdition GetCardEditionByExternalId(CardIdSource cardSource,string id);
        ICard GetCardByIdScryFall(string idScryFall);
        ICardFace GetCardFace(int idCard, string name);
        IEdition GetEditionFromCode(string code);
        IEdition GetEditionById(int idEdition);
        string GetIdScryFall(ICard card, IEdition edition);
        IPicture GetPicture(string idScryFall, bool doNotCache = false);
        IPicture GetDefaultPicture();
        ITreePicture GetTreePicture(string key);
        IRarity GetRarity(string rarity);
        IEdition GetEdition(string sourceName);
        IEdition GetEditionByCode(string code);
        IEdition GetEditionByIdScryFall(string idScryFall);
        ILanguage GetLanguage(int idLanguage);
        ILanguage GetLanguage(string language);
        ILanguage GetDefaultLanguage();
        ILanguage GetEnglishLanguage();
        IBlock GetBlock(string blockName);
        IList<ILanguage> GetLanguages(string idScryFall);
        ICardCollection GetCollection(int collectionId);
        ICardCollection GetCollection(string name);
        ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection);
        ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection, string idScryFall);
        ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, string idScryFall, int idLanguage);
        ICollection<ICardInCollectionCount> GetCardCollectionStatistics(ICard card);
        IPreconstructedDeck GetPreconstructedDeck(int? idEdition, string preconstructedDeckName);
        ICollection<IPreconstructedDeckCardEdition> GetPreconstructedDeckCards(IPreconstructedDeck preconstructedDeck);
        IReadOnlyList<KeyValuePair<string, object>> GetMissingPictureUrls();
        string GetVersoExtension();
        IDisposable BatchMode();
    }
}
