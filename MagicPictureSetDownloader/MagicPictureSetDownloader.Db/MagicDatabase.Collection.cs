namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlServerCe;
    using System.Linq;

    using Common.Database;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        private readonly IDictionary<int, Dictionary<int, ICardInCollectionCount>> _allCardInCollectionCount = new Dictionary<int, Dictionary<int,ICardInCollectionCount>>();
        private readonly IList<ICardCollection> _collections = new List<ICardCollection>();

        public Tuple<ICard, IEdition> GetCardxEdition(int idGatherer)
        {
            CheckReferentialLoaded();
            ICardEdition cardEdition = GetCardEdition(idGatherer);
            if (null == cardEdition)
                return null;

            ICard card = _cards.Values.FirstOrDefault(c => c.Id == cardEdition.IdCard);
            IEdition edition = _editions.FirstOrDefault(e => e.Id == cardEdition.IdEdition);

            if (null == card || null == edition)
                return null;

            return new Tuple<ICard, IEdition>(card, edition);
        }
        public int GetGathererId(ICard card, IEdition edition)
        {
            CheckReferentialLoaded();
            if (card == null || edition == null)
                return 0;

            ICardEdition cardEdition = _cardEditions.Values.FirstOrDefault(ce => ce.IdCard == card.Id && ce.IdEdition == edition.Id);
            return cardEdition == null ? 0 : cardEdition.IdGatherer;
        }
        public IEdition GetEditionFromCode(string code)
        {
            CheckReferentialLoaded();
            return _editions.FirstOrDefault(ed => ed.IsCode(code));
        }

        public ICardCollection GetCollection(string name)
        {
            return _collections.FirstOrDefault(c => c.Name == name);
        }
        public ICollection<ICardCollection> GetAllCollections()
        {
            return (new List<ICardCollection>(_collections).AsReadOnly());
        }

        public IEnumerable<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection)
        {
            Dictionary<int, ICardInCollectionCount> ret;

            if (cardCollection == null)
                return null;

            if (!_allCardInCollectionCount.TryGetValue(cardCollection.Id, out ret))
                return null;

            return ret.Values;
        }
        public ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, int idGatherer)
        {
            Dictionary<int, ICardInCollectionCount> dic;

            if (cardCollection == null)
                return null;

            if (!_allCardInCollectionCount.TryGetValue(cardCollection.Id, out dic))
                return null;

            ICardInCollectionCount ret;
            if (!dic.TryGetValue(idGatherer, out ret))
                return null;

            return ret;
        }
        public ICardCollection InsertNewCollection(string name)
        {
            if (GetCollection(name) != null || string.IsNullOrWhiteSpace(name))
                return null;

            CardCollection collection = new CardCollection { Name = name };
            AddToDbAndUpdateReferential(_connectionString, collection, InsertInReferential);
            return collection;
        }
        public void InsertNewCardInCollection(int idCollection, int idGatherer, int count, int foilCount)
        {
            Dictionary<int, ICardInCollectionCount> collection;

            if (count < 0 || foilCount < 0 || count + foilCount == 0)
                return;

            if (!_allCardInCollectionCount.TryGetValue(idCollection, out collection))
                return;

            if (collection.ContainsKey(idGatherer))
                return;

            CardInCollectionCount cardInCollection = new CardInCollectionCount{IdCollection = idCollection, IdGatherer = idGatherer, Number = count, FoilNumber = foilCount};
            AddToDbAndUpdateReferential(_connectionString, cardInCollection, InsertInReferential);
        }

        public ICardCollection UpdateCollectionName(string oldName, string name)
        {
            return UpdateCollectionName(GetCollection(oldName), name);
        }
        public ICardCollection UpdateCollectionName(ICardCollection collection, string name)
        {
            if (collection == null || string.IsNullOrWhiteSpace(name) || GetCollection(name) != null)
                return collection;

            CardCollection newCollection = collection as CardCollection;

            if (newCollection == null)
                return collection;

            lock (_sync)
            {
                newCollection.Name = name;

                using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
                {
                    cnx.Open();
                    Mapper<CardCollection>.UpdateOne(cnx, newCollection);
                }
            }

            return newCollection;
        }
        public ICardInCollectionCount UpdateCardCollectionCount(ICardInCollectionCount cardInCollection, int count, int countFoil)
        {
            if (cardInCollection == null || count < 0 || countFoil < 0)
                return cardInCollection;

            if (count + countFoil == 0)
            {
                DeleteCardInCollection(cardInCollection.IdCollection, cardInCollection.IdGatherer);
                return null;
            }

            CardInCollectionCount newCardInCollectionCount = cardInCollection as CardInCollectionCount;
            
            if (newCardInCollectionCount == null)
                return cardInCollection;
            
            lock (_sync)
            {
                newCardInCollectionCount.Number = count;
                newCardInCollectionCount.FoilNumber = countFoil;

                using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
                {
                    cnx.Open();
                    Mapper<CardInCollectionCount>.UpdateOne(cnx, newCardInCollectionCount);
                }
            }
            return newCardInCollectionCount;
        }

        public void MoveCollection(string toBeDeletedCollectionName, string toAddCollectionName)
        {
            ICardCollection toBeDeletedCollection = GetCollection(toBeDeletedCollectionName);
            if (toBeDeletedCollection == null)
                return;

            Dictionary<int, ICardInCollectionCount> collectionToRemove;
            if (!_allCardInCollectionCount.TryGetValue(toBeDeletedCollection.Id, out collectionToRemove) || collectionToRemove.Count == 0)
                return;


            ICardCollection toAddCollection = GetCollection(toAddCollectionName);
            if (toAddCollection == null)
                return;

            foreach (ICardInCollectionCount cardInCollectionCount in collectionToRemove.Values)
            {
                ICardInCollectionCount cicc = GetCardCollection(toAddCollection, cardInCollectionCount.IdGatherer);
                if (cicc == null)
                    InsertNewCardInCollection(toAddCollection.Id, cardInCollectionCount.IdGatherer, cardInCollectionCount.Number, cardInCollectionCount.FoilNumber);
                else
                    UpdateCardCollectionCount(cicc, cicc.Number + cardInCollectionCount.Number, cicc.FoilNumber + cardInCollectionCount.FoilNumber);
            }

            DeleteAllCardInCollection(toBeDeletedCollectionName);
        }

        public void DeleteAllCardInCollection(string name)
        {
            ICardCollection cardCollection = GetCollection(name);
            if (cardCollection == null)
                return;

            Dictionary<int, ICardInCollectionCount> collection;

            if (!_allCardInCollectionCount.TryGetValue(cardCollection.Id, out collection) || collection.Count == 0)
                return;

            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
               Mapper<CardInCollectionCount>.DeleteMulti(cnx, collection.Values.Cast<CardInCollectionCount>());
            }

            lock (_sync)
                _allCardInCollectionCount.Remove(cardCollection.Id);
        }
        public void DeleteCardInCollection(int idCollection, int idGatherer)
        {
            Dictionary<int, ICardInCollectionCount> collection;

            if (!_allCardInCollectionCount.TryGetValue(idCollection, out collection))
                return;

            ICardInCollectionCount cardInCollectionCount;
            if (!collection.TryGetValue(idGatherer, out cardInCollectionCount))
                return;

            RemoveFromDbAndUpdateReferential(_connectionString, cardInCollectionCount as CardInCollectionCount, RemoveFromReferential);
        }
        public void DeleteCollection(string name)
        {
            ICardCollection cardCollection = GetCollection(name);
            if (cardCollection == null)
                return;

            RemoveFromDbAndUpdateReferential(_connectionString, cardCollection as CardCollection, RemoveFromReferential);
        }

        private void InsertInReferential(ICardCollection cardCollection)
        {
            _collections.Add(cardCollection);
        }
        private void InsertInReferential(ICardInCollectionCount cardInCollectionCount)
        {
            Dictionary<int, ICardInCollectionCount> dic;
            if (!_allCardInCollectionCount.TryGetValue(cardInCollectionCount.IdCollection, out dic))
            {
                dic = new Dictionary<int, ICardInCollectionCount>();
                _allCardInCollectionCount.Add(cardInCollectionCount.IdCollection, dic);
            }

            dic[cardInCollectionCount.IdGatherer] = cardInCollectionCount;
        }

        private void RemoveFromReferential(ICardCollection cardCollection)
        {
            _collections.Remove(cardCollection);
        }
        private void RemoveFromReferential(ICardInCollectionCount cardInCollectionCount)
        {
            _allCardInCollectionCount[cardInCollectionCount.IdCollection].Remove(cardInCollectionCount.IdGatherer);
        }
    }
}
