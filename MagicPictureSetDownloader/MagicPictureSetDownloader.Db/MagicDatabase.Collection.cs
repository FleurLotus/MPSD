namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Common.Database;
    using Common.Library.Extension;
    using Common.Library.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        private readonly IDictionary<int, IList<ICardInCollectionCount>> _allCardInCollectionCount = new Dictionary<int, IList<ICardInCollectionCount>>();
        private readonly IList<ICardCollection> _collections = new List<ICardCollection>();

        public int GetIdGatherer(ICard card, IEdition edition)
        {
            if (card == null || edition == null)
            {
                return 0;
            }

            CheckReferentialLoaded();
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = _cardEditions.Values.FirstOrDefault(ce => ce.IdCard == card.Id && ce.IdEdition == edition.Id);
                return cardEdition == null ? 0 : cardEdition.IdGatherer;
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
        public ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection, int idGatherer)
        {
            if (cardCollection == null)
            {
                return null;
            }

            return GetCardCollection(cardCollection.Id, idGatherer);
        }
        private ICollection<ICardInCollectionCount> GetCardCollection(int idCollection, int idGatherer)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idGatherer);

                if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
                {
                    return list.Where(cicc => cicc.IdCollection == idCollection && cicc.IdGatherer == idGatherer).ToArray();
                }
                return null;
            }
        }
        public ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, int idGatherer, int idLanguage)
        {
            if (cardCollection == null)
            {
                return null;
            }

            return GetCardCollection(cardCollection.Id, idGatherer, idLanguage);
        }
        private ICardInCollectionCount GetCardCollection(int idCollection, int idGatherer, int idLanguage)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idGatherer);

                if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
                {
                    return list.FirstOrDefault(cicc => cicc.IdCollection == idCollection && cicc.IdGatherer == idGatherer && cicc.IdLanguage == idLanguage);
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
        public void InsertOrUpdateCardInCollection(int idCollection, int idGatherer, int idLanguage, ICardCount cardCount)
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
                    int altArtCountToAdd = cardCount.GetCount(CardCountKeys.AltArt);
                    int foilAltArtCountToAdd = cardCount.GetCount(CardCountKeys.FoilAltArt);

                    ICardInCollectionCount cardInCollection = GetCardCollection(idCollection, idGatherer, idLanguage);
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
                                                                                 IdGatherer = idGatherer,
                                                                                 Number = countToAdd,
                                                                                 FoilNumber = foilCountToAdd,
                                                                                 AltArtNumber = altArtCountToAdd,
                                                                                 FoilAltArtNumber = foilAltArtCountToAdd,
                                                                                 IdLanguage = idLanguage
                                                                             };


                        AddToDbAndUpdateReferential(newCardInCollectionCount, InsertInReferential);

                        AuditAddCard(idCollection, idGatherer, idLanguage, cardCount);
                        return;
                    }

                    //Update
                    int newCount = countToAdd + cardInCollection.Number;
                    int newFoilCount = foilCountToAdd + cardInCollection.FoilNumber;
                    int newAltArtCountToAdd = altArtCountToAdd + cardInCollection.AltArtNumber;
                    int newFoilAltArtCount = foilAltArtCountToAdd + cardInCollection.FoilAltArtNumber;

                    if (newCount < 0 || newFoilCount < 0 || newAltArtCountToAdd < 0 || newFoilAltArtCount < 0)
                    {
                        return;
                    }

                    if (cardInCollection is not CardInCollectionCount updateCardInCollectionCount)
                    {
                        return;
                    }

                    if (newCount + newFoilCount + newAltArtCountToAdd + newFoilAltArtCount == 0)
                    {
                        RemoveFromDbAndUpdateReferential(updateCardInCollectionCount, RemoveFromReferential);

                        AuditAddCard(idCollection, idGatherer, idLanguage, cardCount);

                        return;
                    }

                    updateCardInCollectionCount.Number = newCount;
                    updateCardInCollectionCount.FoilNumber = newFoilCount;
                    updateCardInCollectionCount.AltArtNumber = newAltArtCountToAdd;
                    updateCardInCollectionCount.FoilAltArtNumber = newFoilAltArtCount;

                    using (IDbConnection cnx = _databaseConnection.GetMagicConnection())
                    {
                        Mapper<CardInCollectionCount>.UpdateOne(cnx, updateCardInCollectionCount);
                    }

                    AuditAddCard(idCollection, idGatherer, idLanguage, cardCount);
                }
            }
        }
        public void MoveCardToOtherCollection(ICardCollection collection, int idGatherer, int idLanguage, ICardCount cardCount, ICardCollection collectionDestination)
        {
            if (cardCount == null)
            {
                return;
            }

            foreach (KeyValuePair<ICardCountKey, int> kv in cardCount)
            {
                MoveCardToOtherCollection(collection, idGatherer, idLanguage, kv.Value, kv.Key, collectionDestination);
            }
        }
        private void MoveCardToOtherCollection(ICardCollection collection, int idGatherer, int idLanguage, int countToMove, ICardCountKey cardCountKey, ICardCollection collectionDestination)
        {
            if (countToMove <= 0 || cardCountKey == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                ICardInCollectionCount cardInCollectionCount = GetCardCollection(collection, idGatherer, idLanguage);
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

                InsertOrUpdateCardInCollection(collection.Id, idGatherer, idLanguage, cardCountSource);
                InsertOrUpdateCardInCollection(collectionDestination.Id, idGatherer, idLanguage, cardCountDestination);
            }
        }
        public void MoveCardToOtherCollection(ICardCollection collection, ICard card, IEdition edition, ILanguage language, ICardCount cardCount, ICardCollection collectionDestination)
        {
            if (cardCount == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                int idGatherer = GetIdGatherer(card, edition);
                int idLanguage = language.Id;
                MoveCardToOtherCollection(collection, idGatherer, idLanguage, cardCount, collectionDestination);
            }
        }
        public void ChangeCardEditionFoilAltArtLanguage(ICardCollection collection, ICard card, int countToChange, IEdition editionSource, ICardCountKey cardCountKeySource, ILanguage languageSource,
                                                  IEdition editionDestination, ICardCountKey cardCountKeyDestination, ILanguage languageDestination)
        {
            if (countToChange <= 0)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                if (languageSource == null || languageDestination == null || cardCountKeySource == null || cardCountKeyDestination == null)
                {
                    return;
                }

                int idGathererSource = GetIdGatherer(card, editionSource);
                int idGathererDestination = GetIdGatherer(card, editionDestination);
                ICardInCollectionCount cardInCollectionCount = GetCardCollection(collection, idGathererSource, languageSource.Id);

                if (cardInCollectionCount == null || idGathererDestination == 0)
                {
                    return;
                }

                if (cardInCollectionCount.GetCount(cardCountKeySource) < countToChange)
                {
                    return;
                }

                CardCount cardCountSource = new CardCount
                {
                    { cardCountKeySource, -countToChange }
                };

                CardCount cardCountDestination = new CardCount
                {
                    { cardCountKeyDestination, countToChange }
                };

                InsertOrUpdateCardInCollection(collection.Id, idGathererSource, languageSource.Id, cardCountSource);
                InsertOrUpdateCardInCollection(collection.Id, idGathererDestination, languageDestination.Id, cardCountDestination);
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
                        InsertOrUpdateCardInCollection(toAddCollection.Id, cardInCollectionCount.IdGatherer, cardInCollectionCount.IdLanguage, cardInCollectionCount.GetCardCount());
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
                        AuditAddCard(cardInCollectionCount.IdCollection, cardInCollectionCount.IdGatherer, cardInCollectionCount.IdLanguage, cardCount);

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

                        InsertOrUpdateCardInCollection(collection.Id, card.IdGatherer, idLanguage, cardCount);
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
            //No call to private ICardEdition GetCardEdition(int idGatherer) because call in CheckReferentialLoaded()
            ICardEdition cardEdition = _cardEditions.GetOrDefault(cardInCollectionCount.IdGatherer);

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
            //No call to private ICardEdition GetCardEdition(int idGatherer) because call in CheckReferentialLoaded()
            ICardEdition cardEdition = _cardEditions.GetOrDefault(cardInCollectionCount.IdGatherer);

            if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out IList<ICardInCollectionCount> list))
            {
                list.Remove(cardInCollectionCount);
            }
        }
    }
}
