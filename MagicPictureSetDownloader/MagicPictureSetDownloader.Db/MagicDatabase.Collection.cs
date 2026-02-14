namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Common.Collection;
    using Common.Database;
    using Common.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        private readonly IDictionary<int, IList<ICardInCollectionCount>> _allCardInCollectionCount = new Dictionary<int, IList<ICardInCollectionCount>>();
        private readonly IList<ICardCollection> _collections = new List<ICardCollection>();

        public string GetIdScryFall(ICard card, IEdition edition)
        {
            if (card == null || edition == null)
            {
                return null;
            }

            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = _cardEditions.Values.FirstOrDefault(ce => ce.IdCard == card.Id && ce.IdEdition == edition.Id);
                return cardEdition?.IdScryFall;
            }
        }
        public string[] GetAllIdScryFall(ICard card, IEdition edition)
        {
            if (card == null || edition == null)
            {
                return null;
            }

            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _cardEditions.Values.Where(ce => ce.IdCard == card.Id && ce.IdEdition == edition.Id).Select(ce => ce.IdScryFall).ToArray();
            }
        }
        public string GetIdScryFallByFlavorName(string flavorName, IEdition edition)
        {
            if (string.IsNullOrWhiteSpace(flavorName) || edition == null)
            {
                return null;
            }

            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = _cardEditions.Values.FirstOrDefault(ce => ce.FlavorName == flavorName && ce.IdEdition == edition.Id);
                return cardEdition?.IdScryFall;
            }
        }
        public IEdition GetEditionFromCode(string code)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _editions.FirstOrDefault(ed => ed.IsCode(code));
            }
        }
        public IEdition GetEditionById(int idEdition)
        {
            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                return _editions.FirstOrDefault(ed => ed.Id == idEdition);
            }
        }

        public ICardCollection GetCollection(int collectionId)
        {
            using (new ReaderLock(_lock))
            {
                return _collections.FirstOrDefault(c => c.Id == collectionId);
            }
        }
        public ICardCollection GetCollection(string name)
        {
            using (new ReaderLock(_lock))
            {
                return _collections.FirstOrDefault(c => c.Name == name);
            }
        }
        public ICollection<ICardCollection> GetAllCollections()
        {
            using (new ReaderLock(_lock))
            {
                return (new List<ICardCollection>(_collections).AsReadOnly());
            }
        }

        public ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection)
        {
            if (cardCollection == null)
            {
                return null;
            }

            return GetCardCollection(cardCollection.Id);
        }
        private ICollection<ICardInCollectionCount> GetCardCollection(int idCollection)
        {
            using (new ReaderLock(_lock))
            {
                return _allCardInCollectionCount.SelectMany(kv => kv.Value).Where(cicc => cicc.IdCollection == idCollection).ToArray();
            }
        }
        public ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection, string idScryFall)
        {
            if (cardCollection == null)
            {
                return null;
            }

            return GetCardCollection(cardCollection.Id, idScryFall);
        }
        private ICollection<ICardInCollectionCount> GetCardCollection(int idCollection, string idScryFall)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idScryFall);

                if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
                {
                    return list.Where(cicc => cicc.IdCollection == idCollection && cicc.IdScryFall == idScryFall).ToArray();
                }
                return null;
            }
        }
        public ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, string idScryFall, int idLanguage)
        {
            if (cardCollection == null)
            {
                return null;
            }

            return GetCardCollection(cardCollection.Id, idScryFall, idLanguage);
        }
        private ICardInCollectionCount GetCardCollection(int idCollection, string idScryFall, int idLanguage)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idScryFall);

                if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
                {
                    return list.FirstOrDefault(cicc => cicc.IdCollection == idCollection && cicc.IdScryFall == idScryFall && cicc.IdLanguage == idLanguage);
                }
                return null;
            }
        }
        public ICollection<ICardInCollectionCount> GetCardCollectionStatistics(ICard card)
        {
            using (new ReaderLock(_lock))
            {
                if (_allCardInCollectionCount.TryGetValue(card.Id, out IList<ICardInCollectionCount> list))
                {
                    return new List<ICardInCollectionCount>(list).AsReadOnly();
                }

                return new List<ICardInCollectionCount>();
            }
        }

        public ICardCollection InsertNewCollection(string name)
        {
            using (new WriterLock(_lock))
            {
                if (GetCollection(name) != null || string.IsNullOrWhiteSpace(name))
                {
                    return null;
                }

                CardCollection collection = new CardCollection { Name = name };
                AddToDbAndUpdateReferential(collection, InsertInReferential);
                AuditAddCollection(collection.Id);
                return collection;
            }
        }
        public void InsertOrUpdateCardInCollection(int idCollection, string idScryFall, int idLanguage, ICardCount cardCount)
        {
            if (cardCount == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                using (BatchMode())
                {
                    int countToAdd = cardCount.GetCount(CardCountKeys.Standard);
                    int foilCountToAdd = cardCount.GetCount(CardCountKeys.Foil);

                    ICardInCollectionCount cardInCollection = GetCardCollection(idCollection, idScryFall, idLanguage);
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
            }
        }
        public void MoveCardToOtherCollection(ICardCollection collection, string idScryFall, int idLanguage, ICardCount cardCount, ICardCollection collectionDestination)
        {
            if (cardCount == null)
            {
                return;
            }

            foreach (KeyValuePair<ICardCountKey, int> kv in cardCount)
            {
                MoveCardToOtherCollection(collection, idScryFall, idLanguage, kv.Value, kv.Key, collectionDestination);
            }
        }
        private void MoveCardToOtherCollection(ICardCollection collection, string idScryFall, int idLanguage, int countToMove, ICardCountKey cardCountKey, ICardCollection collectionDestination)
        {
            if (countToMove <= 0 || cardCountKey == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                ICardInCollectionCount cardInCollectionCount = GetCardCollection(collection, idScryFall, idLanguage);
                if (cardInCollectionCount == null)
                {
                    return;
                }

                if (cardInCollectionCount.GetCount(cardCountKey) < countToMove)
                {
                    return;
                }

                CardCount cardCountSource = new CardCount
                {
                    { cardCountKey, -countToMove }
                };

                CardCount cardCountDestination = new CardCount
                {
                    { cardCountKey, countToMove }
                };

                InsertOrUpdateCardInCollection(collection.Id, idScryFall, idLanguage, cardCountSource);
                InsertOrUpdateCardInCollection(collectionDestination.Id, idScryFall, idLanguage, cardCountDestination);
            }
        }
        public ICardCollection UpdateCollectionName(string oldName, string name)
        {
            return UpdateCollectionName(GetCollection(oldName), name);
        }
        public ICardCollection UpdateCollectionName(ICardCollection collection, string name)
        {
            using (new WriterLock(_lock))
            {
                if (collection == null || string.IsNullOrWhiteSpace(name) || GetCollection(name) != null)
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

        public void MoveCollection(string toBeDeletedCollectionName, string toAddCollectionName)
        {
            using (new WriterLock(_lock))
            {
                ICardCollection toBeDeletedCollection = GetCollection(toBeDeletedCollectionName);
                if (toBeDeletedCollection == null)
                {
                    return;
                }

                ICollection<ICardInCollectionCount> collectionToRemove = GetCardCollection(toBeDeletedCollection);
                if (collectionToRemove == null || collectionToRemove.Count == 0)
                {
                    return;
                }

                ICardCollection toAddCollection = GetCollection(toAddCollectionName);
                if (toAddCollection == null)
                {
                    return;
                }

                using (BatchMode())
                {
                    foreach (ICardInCollectionCount cardInCollectionCount in collectionToRemove)
                    {
                        InsertOrUpdateCardInCollection(toAddCollection.Id, cardInCollectionCount.IdScryFall, cardInCollectionCount.IdLanguage, cardInCollectionCount.GetCardCount());
                    }

                    DeleteAllCardInCollection(toBeDeletedCollectionName);
                }
            }
        }

        public void DeleteAllCardInCollection(string name)
        {
            using (new WriterLock(_lock))
            {
                ICardCollection cardCollection = GetCollection(name);
                if (cardCollection == null)
                {
                    return;
                }

                ICollection<ICardInCollectionCount> collection = GetCardCollection(cardCollection);
                if (collection == null || collection.Count == 0)
                {
                    return;
                }

                using (BatchMode())
                {
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
            }
        }
        public void DeleteCollection(string name)
        {
            using (new WriterLock(_lock))
            {
                ICardCollection cardCollection = GetCollection(name);
                if (cardCollection == null)
                {
                    return;
                }

                RemoveFromDbAndUpdateReferential(cardCollection as CardCollection, RemoveFromReferential);
                AuditRemoveCollection(cardCollection.Id);
            }
        }

        public void PreconstructedDeckToCollection(IPreconstructedDeck preconstructedDeck, ICardCollection collection, ILanguage language)
        {
            using (new WriterLock(_lock))
            {
                if (preconstructedDeck == null || collection == null || language == null)
                {
                    return;
                }
                ICollection<IPreconstructedDeckCardEdition> deckComposition = GetPreconstructedDeckCards(preconstructedDeck);
                if (deckComposition == null || deckComposition.Count == 0)
                {
                    return;
                }
                int idLanguage = language.Id;

                using (BatchMode())
                {
                    foreach (IPreconstructedDeckCardEdition card in deckComposition)
                    {
                        CardCount cardCount = new CardCount
                        {
                            { CardCountKeys.Standard, card.Number }
                        };

                        InsertOrUpdateCardInCollection(collection.Id, card.IdScryFall, idLanguage, cardCount);
                    }
                }
            }
        }

        private void InsertInReferential(ICardCollection cardCollection)
        {
            _collections.Add(cardCollection);
        }
        private void InsertInReferential(ICardInCollectionCount cardInCollectionCount)
        {
            //No call to private ICardEdition GetCardEdition(string idScryFall) because call in CheckReferentialLoaded()
            ICardEdition cardEdition = _cardEditions.GetOrDefault(cardInCollectionCount.IdScryFall);

            if (!_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
            {
                list = new List<ICardInCollectionCount>();
                _allCardInCollectionCount.Add(cardEdition.IdCard, list);
            }

            if (list.Contains(cardInCollectionCount))
            {
                throw new Exception("Invalid addition");
            }

            list.Add(cardInCollectionCount);
        }

        private void RemoveFromReferential(ICardCollection cardCollection)
        {
            _collections.Remove(cardCollection);
        }
        private void RemoveFromReferential(ICardInCollectionCount cardInCollectionCount)
        {
            //No call to private ICardEdition GetCardEdition(string idScryFall) because call in CheckReferentialLoaded()
            ICardEdition cardEdition = _cardEditions.GetOrDefault(cardInCollectionCount.IdScryFall);

            if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
            {
                list.Remove(cardInCollectionCount);
            }
        }
    }
}