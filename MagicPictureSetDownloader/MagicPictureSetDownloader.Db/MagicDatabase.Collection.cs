namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

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

            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = _cardEditions.Values.FirstOrDefault(ce => ce.FlavorName == flavorName && ce.IdEdition == edition.Id);
                return cardEdition?.IdScryFall;
            }
        }
        public IEdition GetEditionFromCode(string code)
        {
            using (new ReaderLock(_lock))
            {
                return _editions.FirstOrDefault(ed => ed.IsCode(code));
            }
        }
        public IEdition GetEditionById(int idEdition)
        {
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
                return GetCollectionRead(name);
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
            using (new ReaderLock(_lock))
            {
                return GetCardCollectionRead(cardCollection?.Id);
            }
        }
        public ICollection<ICardInCollectionCount> GetCardCollectionStatistics(ICard card)
        {
            using (new ReaderLock(_lock))
            {
                return GetCardCollectionStatisticsRead(card);
            }
        }

        public ICardCollection InsertNewCollection(string name)
        {
            using (new WriterLock(_lock))
            {
                if (GetCollectionRead(name) != null || string.IsNullOrWhiteSpace(name))
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
            using (BatchMode())
            {
                using (new WriterLock(_lock))
                {
                    InsertOrUpdateCardInCollectionWrite(idCollection, idScryFall, idLanguage, cardCount);
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

            using (BatchMode())
            {
                using (new WriterLock(_lock))
                {
                    ICardInCollectionCount cardInCollectionCount = GetCardCollectionRead(collection?.Id, idScryFall, idLanguage);
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

                    InsertOrUpdateCardInCollectionWrite(collection.Id, idScryFall, idLanguage, cardCountSource);
                    InsertOrUpdateCardInCollectionWrite(collectionDestination.Id, idScryFall, idLanguage, cardCountDestination);
                }
            }
        }
        public ICardCollection UpdateCollectionName(string oldName, string name)
        {
            using (new WriterLock(_lock))
            {
                return UpdateCollectionNameWrite(oldName, name);
            }
        }
        public ICardCollection UpdateCollectionName(ICardCollection collection, string name)
        {
            using (new WriterLock(_lock))
            {
                return UpdateCollectionNameWrite(collection, name);
            }
        }

        public void MoveCollection(string toBeDeletedCollectionName, string toAddCollectionName)
        {
            using (BatchMode())
            {
                using (new WriterLock(_lock))
                {
                    ICardCollection toBeDeletedCollection = GetCollectionRead(toBeDeletedCollectionName);
                    if (toBeDeletedCollection == null)
                    {
                        return;
                    }

                    ICollection<ICardInCollectionCount> collectionToRemove = GetCardCollectionRead(toBeDeletedCollection.Id);
                    if (collectionToRemove == null || collectionToRemove.Count == 0)
                    {
                        return;
                    }

                    ICardCollection toAddCollection = GetCollectionRead(toAddCollectionName);
                    if (toAddCollection == null)
                    {
                        return;
                    }

                    foreach (ICardInCollectionCount cardInCollectionCount in collectionToRemove)
                    {
                        InsertOrUpdateCardInCollectionWrite(toAddCollection.Id, cardInCollectionCount.IdScryFall, cardInCollectionCount.IdLanguage, cardInCollectionCount.GetCardCount());
                    }

                    DeleteAllCardInCollectionWrite(toBeDeletedCollectionName);
                }
            }
        }
        public void DeleteAllCardInCollection(string name)
        {
            using (BatchMode())
            {
                using (new WriterLock(_lock))
                {
                    DeleteAllCardInCollectionWrite(name);
                }
            }
        }
        public void DeleteCollection(string name)
        {
            using (new WriterLock(_lock))
            {
                ICardCollection cardCollection = GetCollectionRead(name);
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
            using (BatchMode())
            {
                using (new WriterLock(_lock))
                {
                    if (preconstructedDeck == null || collection == null || language == null)
                    {
                        return;
                    }
                    ICollection<IPreconstructedDeckCardEdition> deckComposition = GetPreconstructedDeckCardsRead(preconstructedDeck.Id);
                    if (deckComposition == null || deckComposition.Count == 0)
                    {
                        return;
                    }
                    int idLanguage = language.Id;

                    foreach (IPreconstructedDeckCardEdition card in deckComposition)
                    {
                        CardCount cardCount = new CardCount
                        {
                            { CardCountKeys.Standard, card.Number }
                        };

                        InsertOrUpdateCardInCollectionWrite(collection.Id, card.IdScryFall, idLanguage, cardCount);
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
            ICardEdition cardEdition = GetCardEditionRead(cardInCollectionCount.IdScryFall);

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
            ICardEdition cardEdition = GetCardEditionRead(cardInCollectionCount.IdScryFall);

            if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
            {
                list.Remove(cardInCollectionCount);
            }
        }
    }
}