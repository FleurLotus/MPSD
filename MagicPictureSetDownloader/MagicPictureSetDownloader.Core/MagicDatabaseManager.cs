namespace MagicPictureSetDownloader.Core
{
    using System;
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

        public ICard GetCard(string name)
        {
            return DbInstance.GetCard(name);
        }
        public IPicture GetPicture(int idGatherer)
        {
            return DbInstance.GetPicture(idGatherer);
        }
        public IEdition GetEdition(string sourceName)
        {
            return DbInstance.GetEdition(sourceName);
        }
        public void InsertNewPicture(int idGatherer, byte[] data)
        {
            DbInstance.InsertNewPicture(idGatherer, data);
        }
        public void InsertNewCard(string name, string text, string power, string toughness, string castingcost, int? loyalty, string type)
        {
            DbInstance.InsertNewCard(name, text, power, toughness, castingcost, loyalty, type);
        }
        public void InsertNewCardEdition(int idGatherer, int idEdition, string name, string rarity)
        {
            DbInstance.InsertNewCardEdition(idGatherer, idEdition, name, rarity);
        }
    }
}
