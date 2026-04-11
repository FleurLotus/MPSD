namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Common.Collection;
    using Common.Database;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        //Those methods expect the read lock or higher to be taken before
        private ICard GetCardRead(string name)
        {
            return _cards.GetOrDefault(name) ?? _cardNameSimple.GetOrDefault(name);
        }
        private ICardFace GetCardFaceRead(int idCard, string name)
        {
            ICard card = _cardsbyId.GetOrDefault(idCard);
            if (card != null)
            {
                if (card.MainCardFace?.Name == name)
                {
                    return card.MainCardFace;
                }
                if (card.OtherCardFace?.Name == name)
                {
                    return card.OtherCardFace;
                }
            }
            return null;
        }
        private IBlock GetBlockRead(string blockName)
        {
            return _blocks.Values.FirstOrDefault(b => string.Compare(b.Name, blockName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
        private ICardEdition GetCardEditionRead(string idScryFall)
        {
            return _cardEditions.GetOrDefault(idScryFall);
        }
        private IEdition GetEditionRead(string sourceName)
        {
            return _editions.FirstOrDefault(ed => string.Equals(ed.Name, sourceName, StringComparison.InvariantCultureIgnoreCase));
        }
        private IEdition GetEditionByCodeRead(string code)
        {
            return _editions.FirstOrDefault(ed => string.Equals(ed.Code, code, StringComparison.InvariantCultureIgnoreCase));
        }

        private ICard GetCardByIdScryFallRead(string idScryFall)
        {
            ICardEdition cardEdition = GetCardEditionRead(idScryFall);
            if (cardEdition == null)
            {
                return null;
            }

            return _cardsbyId.GetOrDefault(cardEdition.IdCard);
        }

        private ILanguage GetLanguageRead(string language)
        {
            if (_languages.TryGetValue(language, out ILanguage lang) && lang != null)
            {
                return lang;
            }

            if (_alternativeNameLanguages.TryGetValue(language, out lang) && lang != null)
            {
                return lang;
            }
            return null;
        }
        private ICardCollection GetCollectionRead(string name)
        {
            return _collections.FirstOrDefault(c => c.Name == name);
        }

        private ICollection<ICardInCollectionCount> GetCardCollectionRead(int? idCollection)
        {
            if (!idCollection.HasValue)
            {
                return null;
            }

            return _allCardInCollectionCount.SelectMany(kv => kv.Value).Where(cicc => cicc.IdCollection == idCollection).ToArray();
        }
        private ICardInCollectionCount GetCardCollectionRead(int? idCollection, string idScryFall, int idLanguage)
        {
            if (!idCollection.HasValue)
            {
                return null;
            }
            ICardEdition cardEdition = GetCardEditionRead(idScryFall);

            if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
            {
                return list.FirstOrDefault(cicc => cicc.IdCollection == idCollection && cicc.IdScryFall == idScryFall && cicc.IdLanguage == idLanguage);
            }
            return null;
        }
        private IPreconstructedDeck GetPreconstructedDeckRead(int? idEdition, string preconstructedDeckName)
        {
            return _preconstructedDecks.Values.FirstOrDefault(pd => pd.IdEdition == idEdition && string.Compare(pd.Name, preconstructedDeckName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
        private ICollection<IPreconstructedDeckCardEdition> GetPreconstructedDeckCardsRead(int? idPreconstructedDeck)
        {
            if (!idPreconstructedDeck.HasValue)
            {
                return Array.Empty<IPreconstructedDeckCardEdition>();
            }

            if (_preconstructedDeckCards.TryGetValue(idPreconstructedDeck.Value, out IList<IPreconstructedDeckCardEdition> preconstructedDeckCards))
            {
                return preconstructedDeckCards.ToArray();
            }
            return Array.Empty<IPreconstructedDeckCardEdition>();
        }
        private IPreconstructedDeckCardEdition GetPreconstructedDeckCardRead(int idPreconstructedDeck, string idScryFall)
        {
            ICollection<IPreconstructedDeckCardEdition> preconstructedDeckCards = GetPreconstructedDeckCardsRead(idPreconstructedDeck);
            if (preconstructedDeckCards == null)
            {
                return null;
            }

            return preconstructedDeckCards.FirstOrDefault(pdc => pdc.IdScryFall == idScryFall);
        }
        private IPreconstructedDeck GetPreconstructedDeckRead(int idPreconstructedDeck)
        {
            if (_preconstructedDecks.TryGetValue(idPreconstructedDeck, out IPreconstructedDeck preconstructedDeck))
            {
                return preconstructedDeck;
            }
            return null;
        }
        private IList<IOption> GetOptionsRead(TypeOfOption type)
        {
            if (!_allOptions.TryGetValue(type, out IList<IOption> options))
            {
                return null;
            }

            return new List<IOption>(options).AsReadOnly();
        }
        private IOption GetOptionRead(TypeOfOption type, string key)
        {
            IList<IOption> options = GetOptionsRead(type);
            return options?.FirstOrDefault(o => o.Key == key);
        }
        private IRarity GetRarityRead(string rarity)
        {
            return _rarities.GetOrDefault(rarity);
        }
        private ICollection<ICardInCollectionCount> GetCardCollectionStatisticsRead(ICard card)
        {
            if (_allCardInCollectionCount.TryGetValue(card.Id, out IList<ICardInCollectionCount> list))
            {
                return new List<ICardInCollectionCount>(list).AsReadOnly();
            }

            return new List<ICardInCollectionCount>();
        }

        //Those methods expect the write lock to be taken before
        private void InsertOrUpdateCardInCollectionWrite(int idCollection, string idScryFall, int idLanguage, ICardCount cardCount)
        {
            int countToAdd = cardCount.GetCount(CardCountKeys.Standard);
            int foilCountToAdd = cardCount.GetCount(CardCountKeys.Foil);

            ICardInCollectionCount cardInCollection = GetCardCollectionRead(idCollection, idScryFall, idLanguage);
            if (cardInCollection == null)
            {
                //Insert new 
                if (cardCount.Any(kv => kv.Value < 0) || cardCount.GetTotalCount() == 0)
                {
                    return;
                }

                CardInCollectionCount newCardInCollectionCount = new CardInCollectionCount
                {
                    IdCollection = idCollection,
                    IdScryFall = idScryFall,
                    Number = countToAdd,
                    FoilNumber = foilCountToAdd,
                    IdLanguage = idLanguage
                };

                AddToDbAndUpdateReferential(newCardInCollectionCount, InsertInReferential);

                AuditAddCard(idCollection, idScryFall, idLanguage, cardCount);
                return;
            }

            //Update
            int newCount = countToAdd + cardInCollection.Number;
            int newFoilCount = foilCountToAdd + cardInCollection.FoilNumber;

            if (newCount < 0 || newFoilCount < 0)
            {
                return;
            }

            if (cardInCollection is not CardInCollectionCount updateCardInCollectionCount)
            {
                return;
            }

            if (newCount + newFoilCount == 0)
            {
                RemoveFromDbAndUpdateReferential(updateCardInCollectionCount, RemoveFromReferential);

                AuditAddCard(idCollection, idScryFall, idLanguage, cardCount);

                return;
            }

            updateCardInCollectionCount.Number = newCount;
            updateCardInCollectionCount.FoilNumber = newFoilCount;

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
            {
                Mapper<CardInCollectionCount>.UpdateOne(cnx, updateCardInCollectionCount);
            }

            AuditAddCard(idCollection, idScryFall, idLanguage, cardCount);
        }
        private void DeleteAllCardInCollectionWrite(string name)
        {
            ICardCollection cardCollection = GetCollectionRead(name);
            if (cardCollection == null)
            {
                return;
            }

            ICollection<ICardInCollectionCount> collection = GetCardCollectionRead(cardCollection.Id);
            if (collection == null || collection.Count == 0)
            {
                return;
            }

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
            {
                Mapper<CardInCollectionCount>.DeleteMulti(cnx, collection.Cast<CardInCollectionCount>());
            }

            foreach (ICardInCollectionCount cardInCollectionCount in collection)
            {
                ICardCount cardCount = new CardCount();
                foreach (KeyValuePair<ICardCountKey, int> kv in cardInCollectionCount.GetCardCount())
                {
                    cardCount.Add(kv.Key, -kv.Value);
                }
                AuditAddCard(cardInCollectionCount.IdCollection, cardInCollectionCount.IdScryFall, cardInCollectionCount.IdLanguage, cardCount);

                RemoveFromReferential(cardInCollectionCount);
            }
        }
        private void InsertNewCardFaceWrite(ICard card, ICardFace cardFace)
        {
            if (card == null)
            {
                return;
            }

            if (!card.HasCardFace(cardFace.Name))
            {
                AddToDbAndUpdateReferential((CardFace) cardFace, InsertInReferential);
            }
        }
        private ICardCollection UpdateCollectionNameWrite(ICardCollection collection, string name)
        {
            if (collection == null || string.IsNullOrWhiteSpace(name) || GetCollectionRead(name) != null)
            {
                return collection;
            }

            if (collection is not CardCollection newCollection)
            {
                return collection;
            }

            newCollection.Name = name;

            using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
            {
                Mapper<CardCollection>.UpdateOne(cnx, newCollection);
            }

            return newCollection;
        }
    }
}