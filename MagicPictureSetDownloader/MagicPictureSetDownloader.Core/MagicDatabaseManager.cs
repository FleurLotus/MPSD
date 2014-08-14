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
        public ICard GetCard(string name)
        {
            return DbInstance.GetCard(name);
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
        public void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type)
        {
            DbInstance.InsertNewCard(name, text, power, toughness, castingcost, loyalty, type);
        }
        public void InsertNewCardEdition(int idGatherer, int idEdition, string name, string rarity, string url)
        {
            DbInstance.InsertNewCardEdition(idGatherer, idEdition, name, rarity, url);
        }
    }
}
