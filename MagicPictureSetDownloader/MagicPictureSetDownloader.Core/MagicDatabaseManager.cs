﻿namespace MagicPictureSetDownloader.Core
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
        public ICollection<ICardAllDbInfo> GetAllInfos()
        {
            return DbInstance.GetAllInfos();
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
    }
}
