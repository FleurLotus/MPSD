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
        ICard GetCard(string name, string partName);
        ICard GetCard(int idGatherer);
        IEdition GetEditionFromCode(string code);
        IEdition GetEditionById(int idEdition);
        int GetIdGatherer(ICard card, IEdition edition);
        IPicture GetPicture(int idGatherer, bool doNotCache = false);
        IPicture GetDefaultPicture();
        ITreePicture GetTreePicture(string key);
        IRarity GetRarity(string rarity);
        IEdition GetEdition(string sourceName);
        IEdition GetEdition(int idGatherer);
        ILanguage GetLanguage(int idLanguage);
        ICardEdition GetCardEditionFromPictureUrl(string pictureUrl);
        ICardEditionVariation GetCardEditionVariationFromPictureUrl(string pictureUrl);
        IList<ICardEditionVariation> GetCardEditionVariation(int idGatherer);
        ILanguage GetDefaultLanguage();
        ILanguage GetEnglishLanguage();
        IBlock GetBlock(string blockName);
        IList<ILanguage> GetLanguages(int idGatherer);
        ICardCollection GetCollection(int collectionId);
        ICardCollection GetCollection(string name);
        ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection);
        ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection, int idGatherer);
        ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, int idGatherer, int idLanguage);
        ICollection<ICardInCollectionCount> GetCardCollectionStatistics(ICard card);
        IPreconstructedDeck GetPreconstructedDeck(int idEdition, string preconstructedDeckName);
        ICollection<IPreconstructedDeckCardEdition> GetPreconstructedDeckCards(IPreconstructedDeck preconstructedDeck);

        string[] GetMissingPictureUrls();
        ICardAllDbInfo[] GetCardsWithPicture();
        int[] GetRulesId();
        IDisposable BatchMode();
    }
}
