namespace MagicPictureSetDownloader.Db
{
    using System.Collections.Generic;
    using System.Data.SqlServerCe;
    using System.Linq;

    using Common.Database;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    public partial class MagicDatabase
    {
        private readonly IDictionary<int, List<ICardInCollectionCount>> _allCardInCollectionCount = new Dictionary<int, List<ICardInCollectionCount>>();
        private readonly IList<ICardCollection> _collections = new List<ICardCollection>();

        public ICardCollection GetCollection(string name)
        {
            return _collections.FirstOrDefault(c => c.Name == name);
        }
        public ICollection<ICardCollection> GetAllCollections()
        {
            return (new List<ICardCollection>(_collections).AsReadOnly());
        }

        public IList<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection)
        {
            List<ICardInCollectionCount> ret;

            if (cardCollection == null)
                return null;

            if (!_allCardInCollectionCount.TryGetValue(cardCollection.Id, out ret))
                return null;

            return ret.AsReadOnly();
        }
        private ICardInCollectionCount GetCardCollection(ICardCollection cardCollection, int idGatherer)
        {
            List<ICardInCollectionCount> ret;

            if (cardCollection == null)
                return null;

            if (!_allCardInCollectionCount.TryGetValue(cardCollection.Id, out ret))
                return null;

            return ret.FirstOrDefault(cicc => cicc.IdGatherer == idGatherer);
        }
        public void InsertNewCollection(string name)
        {
            if (GetCollection(name) != null || string.IsNullOrWhiteSpace(name))
                return;

            CardCollection collection = new CardCollection { Name = name };
            AddToDbAndUpdateReferential(_connectionString, collection, InsertInReferential);
        }
        public void InsertNewCardInCollection(int idCollection, int idGatherer, int count, int foilCount)
        {
            List<ICardInCollectionCount> collection;

            if (count < 0 || foilCount < 0 || count + foilCount == 0)
                return;

            if (!_allCardInCollectionCount.TryGetValue(idCollection, out collection))
                return;

            if (collection.FirstOrDefault(c => c.IdGatherer == idGatherer) != null)
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

            List<ICardInCollectionCount> collectionToRemove;
            if (!_allCardInCollectionCount.TryGetValue(toBeDeletedCollection.Id, out collectionToRemove) || collectionToRemove.Count == 0)
                return;


            ICardCollection toAddCollection = GetCollection(toAddCollectionName);
            if (toAddCollection == null)
                return;

            foreach (ICardInCollectionCount cardInCollectionCount in collectionToRemove)
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

            List<ICardInCollectionCount> collection;

            if (!_allCardInCollectionCount.TryGetValue(cardCollection.Id, out collection) || collection.Count == 0)
                return;

            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
               Mapper<CardInCollectionCount>.DeleteMulti(cnx, collection.Cast<CardInCollectionCount>());
            }

            lock (_sync)
                _allCardInCollectionCount.Remove(cardCollection.Id);
        }
        public void DeleteCardInCollection(int idCollection, int idGatherer)
        {
            List<ICardInCollectionCount> collection;

            if (!_allCardInCollectionCount.TryGetValue(idCollection, out collection))
                return;

            ICardInCollectionCount cardInCollectionCount = collection.FirstOrDefault(c => c.IdGatherer == idGatherer);
            if (cardInCollectionCount == null)
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
            List<ICardInCollectionCount> list;
            if (!_allCardInCollectionCount.TryGetValue(cardInCollectionCount.IdCollection, out list))
            {
                list = new List<ICardInCollectionCount>();
                _allCardInCollectionCount.Add(cardInCollectionCount.IdCollection, list);
            }

            list.Add(cardInCollectionCount);
        }

        private void RemoveFromReferential(ICardCollection cardCollection)
        {
            _collections.Remove(cardCollection);
        }
        private void RemoveFromReferential(ICardInCollectionCount cardInCollectionCount)
        {
            _allCardInCollectionCount[cardInCollectionCount.IdCollection].Remove(cardInCollectionCount);
        }
    }
}
