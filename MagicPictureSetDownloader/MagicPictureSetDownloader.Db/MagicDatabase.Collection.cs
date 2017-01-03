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
    using MagicPictureSetDownloader.DbGenerator;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        private readonly IDictionary<int, IList<ICardInCollectionCount>> _allCardInCollectionCount = new Dictionary<int, IList<ICardInCollectionCount>>();
        private readonly IList<ICardCollection> _collections = new List<ICardCollection>();

        public int GetIdGatherer(ICard card, IEdition edition)
        {
            if (card == null || edition == null)
                return 0;

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
                return _editions.FirstOrDefault(ed => ed.IsCode(code));
        }

        public ICardCollection GetCollection(int collectionId)
        {
            using (new ReaderLock(_lock))
                return _collections.FirstOrDefault(c => c.Id == collectionId);
        }
        public ICardCollection GetCollection(string name)
        {
            using (new ReaderLock(_lock))
                return _collections.FirstOrDefault(c => c.Name == name);
        }
        public ICollection<ICardCollection> GetAllCollections()
        {
            using (new ReaderLock(_lock))
                return (new List<ICardCollection>(_collections).AsReadOnly());
        }

        public ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection)
        {
            if (cardCollection == null)
                return null;

            return GetCardCollection(cardCollection.Id);
        }
        private ICollection<ICardInCollectionCount> GetCardCollection(int idCollection)
        {
            using (new ReaderLock(_lock))
                return _allCardInCollectionCount.SelectMany(kv => kv.Value).Where(cicc => cicc.IdCollection == idCollection).ToArray();
        }
        public ICollection<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection, int idGatherer)
        {
            if (cardCollection == null)
                return null;

            return GetCardCollection(cardCollection.Id, idGatherer);
        }
        private ICollection<ICardInCollectionCount> GetCardCollection(int idCollection, int idGatherer)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idGatherer);

                IList<ICardInCollectionCount> list;
                if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out list))
                {
                    return list.Where(cicc => cicc.IdCollection == idCollection && cicc.IdGatherer == idGatherer).ToArray();
                }
                return null;
            }
        }
        public ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, int idGatherer, int idLanguage)
        {
            if (cardCollection == null)
                return null;

            return GetCardCollection(cardCollection.Id, idGatherer, idLanguage);
        }
        private ICardInCollectionCount GetCardCollection(int idCollection, int idGatherer, int idLanguage)
        {
            using (new ReaderLock(_lock))
            {
                ICardEdition cardEdition = GetCardEdition(idGatherer);

                IList<ICardInCollectionCount> list;
                if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out list))
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
                IList<ICardInCollectionCount> list;
                if (_allCardInCollectionCount.TryGetValue(card.Id, out list))
                    return new List<ICardInCollectionCount>(list).AsReadOnly();
                return new List<ICardInCollectionCount>();
            }
        }

        public ICardCollection InsertNewCollection(string name)
        {
            using (new WriterLock(_lock))
            {
                if (GetCollection(name) != null || string.IsNullOrWhiteSpace(name))
                    return null;

                CardCollection collection = new CardCollection { Name = name };
                AddToDbAndUpdateReferential(DatabaseType.Data, collection, InsertInReferential);
                AuditAddCollection(collection.Id);
                return collection;
            }
        }
        public void InsertOrUpdateCardInCollection(int idCollection, int idGatherer, int idLanguage, int countToAdd, int foilCountToAdd)
        {
            using (new WriterLock(_lock))
            {
                using (BatchMode())
                {
                    ICardInCollectionCount cardInCollection = GetCardCollection(idCollection, idGatherer, idLanguage);
                    if (cardInCollection == null)
                    {
                        //Insert new 
                        if (countToAdd < 0 || foilCountToAdd < 0 || countToAdd + foilCountToAdd == 0)
                            return;

                        CardInCollectionCount newCardInCollectionCount = new CardInCollectionCount
                                                                             {
                                                                                 IdCollection = idCollection,
                                                                                 IdGatherer = idGatherer,
                                                                                 Number = countToAdd,
                                                                                 FoilNumber = foilCountToAdd,
                                                                                 IdLanguage = idLanguage
                                                                             };


                        AddToDbAndUpdateReferential(DatabaseType.Data, newCardInCollectionCount, InsertInReferential);

                        AuditAddCard(idCollection, idGatherer, idLanguage, false, countToAdd);
                        AuditAddCard(idCollection, idGatherer, idLanguage, true, foilCountToAdd);
                        return;
                    }

                    //Update
                    int newCount = countToAdd + cardInCollection.Number;
                    int newFoilCount = foilCountToAdd + cardInCollection.FoilNumber;

                    if (newCount < 0 || newFoilCount < 0)
                        return;

                    CardInCollectionCount updateCardInCollectionCount = cardInCollection as CardInCollectionCount;
                    if (updateCardInCollectionCount == null)
                        return;

                    if (newCount + newFoilCount == 0)
                    {
                        RemoveFromDbAndUpdateReferential(DatabaseType.Data, updateCardInCollectionCount, RemoveFromReferential);

                        AuditAddCard(idCollection, idGatherer, idLanguage, false, countToAdd);
                        AuditAddCard(idCollection, idGatherer, idLanguage, true, foilCountToAdd);
                        return;
                    }

                    updateCardInCollectionCount.Number = newCount;
                    updateCardInCollectionCount.FoilNumber = newFoilCount;

                    using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
                    {
                        Mapper<CardInCollectionCount>.UpdateOne(cnx, updateCardInCollectionCount);
                    }
                    AuditAddCard(idCollection, idGatherer, idLanguage, false, countToAdd);
                    AuditAddCard(idCollection, idGatherer, idLanguage, true, foilCountToAdd);
                }
            }
        }
        public void MoveCardToOtherCollection(ICardCollection collection, int idGatherer, int idLanguage, int countToMove, bool isFoil, ICardCollection collectionDestination)
        {
            if (countToMove <= 0)
                return;

            using (new WriterLock(_lock))
            {
                ICardInCollectionCount cardInCollectionCount = GetCardCollection(collection, idGatherer, idLanguage);
                if (cardInCollectionCount == null)
                    return;

                int count = 0;
                int foilCount = 0;

                if (isFoil)
                {
                    foilCount = countToMove;

                    if (cardInCollectionCount.FoilNumber < countToMove)
                        return;
                }
                else
                {
                    count = countToMove;

                    if (cardInCollectionCount.Number < countToMove)
                        return;
                }

                InsertOrUpdateCardInCollection(collection.Id, idGatherer, idLanguage, -count, -foilCount);
                InsertOrUpdateCardInCollection(collectionDestination.Id, idGatherer, idLanguage, count, foilCount);
            }
        }
        public void MoveCardToOtherCollection(ICardCollection collection, ICard card, IEdition edition, ILanguage language, int countToMove, bool isFoil, ICardCollection collectionDestination)
        {
            if (countToMove <= 0)
                return;

            using (new WriterLock(_lock))
            {
                int idGatherer = GetIdGatherer(card, edition);
                int idLanguage = language.Id;
                MoveCardToOtherCollection(collection, idGatherer, idLanguage, countToMove, isFoil, collectionDestination);
            }
        }
        public void ChangeCardEditionFoilLanguage(ICardCollection collection, ICard card, int countToChange, IEdition editionSource, bool isFoilSource, ILanguage languageSource,
                                                  IEdition editionDestination, bool isFoilDestination, ILanguage languageDestination)
        {
            if (countToChange <= 0)
                return;

            using (new WriterLock(_lock))
            {
                if (languageSource == null || languageDestination == null)
                    return;

                int idGathererSource = GetIdGatherer(card, editionSource);
                int idGathererDestination = GetIdGatherer(card, editionDestination);
                ICardInCollectionCount cardInCollectionCount = GetCardCollection(collection, idGathererSource, languageSource.Id);
                if (cardInCollectionCount == null || idGathererDestination <= 0)
                    return;

                int count = 0;
                int foilCount = 0;
                int countDestination = 0;
                int foilCountDestination = 0;

                if (isFoilSource)
                {
                    foilCount = -countToChange;

                    if (cardInCollectionCount.FoilNumber < countToChange)
                        return;
                }
                else
                {
                    count = -countToChange;

                    if (cardInCollectionCount.Number < countToChange)
                        return;
                }

                if (isFoilDestination)
                    foilCountDestination = countToChange;
                else
                    countDestination = countToChange;


                InsertOrUpdateCardInCollection(collection.Id, idGathererSource, languageSource.Id, count, foilCount);
                InsertOrUpdateCardInCollection(collection.Id, idGathererDestination, languageDestination.Id, countDestination, foilCountDestination);
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
                    return collection;

                CardCollection newCollection = collection as CardCollection;

                if (newCollection == null)
                    return collection;

                newCollection.Name = name;

                using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
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
                    return;

                ICollection<ICardInCollectionCount> collectionToRemove = GetCardCollection(toBeDeletedCollection);
                if (collectionToRemove == null || collectionToRemove.Count == 0)
                    return;

                ICardCollection toAddCollection = GetCollection(toAddCollectionName);
                if (toAddCollection == null)
                    return;
 
                using (BatchMode())
                {
                    foreach (ICardInCollectionCount cardInCollectionCount in collectionToRemove)
                    {
                        InsertOrUpdateCardInCollection(toAddCollection.Id, cardInCollectionCount.IdGatherer, cardInCollectionCount.IdLanguage, cardInCollectionCount.Number, cardInCollectionCount.FoilNumber);
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
                    return;

                ICollection<ICardInCollectionCount> collection = GetCardCollection(cardCollection);
                if (collection == null || collection.Count == 0)
                    return;

                using (BatchMode())
                {
                    using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
                    {
                        Mapper<CardInCollectionCount>.DeleteMulti(cnx, collection.Cast<CardInCollectionCount>());
                    }

                    foreach (ICardInCollectionCount cardInCollectionCount in collection)
                    {
                        AuditAddCard(cardInCollectionCount.IdCollection, cardInCollectionCount.IdGatherer, cardInCollectionCount.IdLanguage, false, -cardInCollectionCount.Number);
                        AuditAddCard(cardInCollectionCount.IdCollection, cardInCollectionCount.IdGatherer, cardInCollectionCount.IdLanguage, true, -cardInCollectionCount.FoilNumber);

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
                    return;

                RemoveFromDbAndUpdateReferential(DatabaseType.Data, cardCollection as CardCollection, RemoveFromReferential);
                AuditRemoveCollection(cardCollection.Id);
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

            IList<ICardInCollectionCount> list;
            if (!_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out list))
            {
                list = new List<ICardInCollectionCount>();
                _allCardInCollectionCount.Add(cardEdition.IdCard, list);
            }

            if (list.Contains(cardInCollectionCount))
                throw new Exception("Invalid addition");

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

            IList<ICardInCollectionCount> list;
            if (_allCardInCollectionCount.TryGetValue(cardEdition.IdCard, out list))
            {
                list.Remove(cardInCollectionCount);
            }
        }
    }
}
