namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Db;

    public class MagicDatabaseManager
    {
        private static readonly Lazy<MagicDatabase> _lazyIntance = new Lazy<MagicDatabase>(() => new MagicDatabase("MagicData.sdf", "MagicPicture.sdf"));

        private MagicDatabase DbInstance
        {
            get
            {
                return _lazyIntance.Value;
            }
        }
        public ICollection<ICardAllDbInfo> GetAllInfos(bool withCollectionInfo, int onlyInCollectionId)
        {
            return DbInstance.GetAllInfos(withCollectionInfo, onlyInCollectionId);
        }
        public IList<IOption> GetOptions(TypeOfOption type)
        {
            return DbInstance.GetOptions(type);
        }
        public ICard GetCard(string name, string partName)
        {
            return DbInstance.GetCard(name, partName);
        }
        public IPicture GetPicture(int idGatherer)
        {
            return DbInstance.GetPicture(idGatherer);
        }
        public ITreePicture GetTreePicture(string key)
        {
            return DbInstance.GetTreePicture(key);
        }
        public IPicture GetDefaultPicture()
        {
            return DbInstance.GetDefaultPicture();
        }
        public IEdition GetEdition(string sourceName)
        {
            return DbInstance.GetEdition(sourceName);
        }
        public IOption GetOption(TypeOfOption type, string key)
        {
            return DbInstance.GetOption(type, key);
        }
        public ICardCollection GetCollection(string name)
        {
            return DbInstance.GetCollection(name);
        }
        public ICollection<ICardCollection> GetAllCollections()
        {
            return DbInstance.GetAllCollections();
        }

        public IList<ICardInCollectionCount> GetCardCollection(ICardCollection cardCollection)
        {
            return DbInstance.GetCardCollection(cardCollection);
        }
        
        public string[] GetMissingPictureUrls()
        {
            return DbInstance.GetMissingPictureUrls();
        }
        public void InsertNewPicture(int idGatherer, byte[] data)
        {
            DbInstance.InsertNewPicture(idGatherer, data);
        }
        public void InsertNewTreePicture(string key, byte[] data)
        {
            DbInstance.InsertNewTreePicture(key, data);
        }
        public void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type, string partName, string otherPartName)
        {
            DbInstance.InsertNewCard(name, text, power, toughness, castingcost, loyalty, type, partName, otherPartName);
        }
        public void InsertNewCardEdition(int idGatherer, int idEdition, string name, string partName, string rarity, string url)
        {
            DbInstance.InsertNewCardEdition(idGatherer, idEdition, name, partName, rarity, url);
        }
        public void InsertNewOption(TypeOfOption type, string key, string value)
        {
            DbInstance.InsertNewOption(type, key, value);
        }
        public void InsertNewCollection(string name)
        {
            DbInstance.InsertNewCollection(name);
        }
        public void InsertNewCardInCollection(int idCollection, int idGatherer, int count, int foilCount = 0)
        {
            DbInstance.InsertNewCardInCollection(idCollection, idGatherer, count, foilCount);
        }

        public void EditionCompleted(int editionId)
        {
            DbInstance.EditionCompleted(editionId);
        }
        public ICardCollection UpdateCollectionName(ICardCollection collection, string name)
        {
            return DbInstance.UpdateCollectionName(collection, name);
        }
        public ICardCollection UpdateCollectionName(string oldName, string name)
        {
            return DbInstance.UpdateCollectionName(oldName, name);
        }

        public ICardInCollectionCount UpdateCardCollectionCount(ICardInCollectionCount cardInCollection, int count, int countFoil = 0)
        {
            return DbInstance.UpdateCardCollectionCount(cardInCollection, count, countFoil);
        }

        public void DeleteAllCardInCollection(string name)
        {
            DbInstance.DeleteAllCardInCollection(name);
        }
        public void MoveCollection(string toBeDeletedCollectionName, string toAddCollectionName)
        {
            DbInstance.MoveCollection(toBeDeletedCollectionName, toAddCollectionName);
        }
        public void DeleteCardInCollection(int idCollection, int idGatherer)
        {
            DbInstance.DeleteCardInCollection(idCollection, idGatherer);
        }
        public void DeleteCollection(string name)
        {
            DbInstance.DeleteCollection(name);
        }
    }
}
