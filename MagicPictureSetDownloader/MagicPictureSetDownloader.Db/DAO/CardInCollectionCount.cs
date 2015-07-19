namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics.CodeAnalysis;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable(Name = "CardEditionsInCollection")]
    internal class CardInCollectionCount : ICardInCollectionCount
    {
        [DbColumn]
        [DbKeyColumn(false)]
        public int IdCollection { get; set; }
        [DbColumn]
        [DbKeyColumn(false)]
        public int IdGatherer { get; set; }
        [DbColumn]
        public int Number { get; set; }
        [DbColumn]
        public int FoilNumber { get; set; }
        [DbColumn]
        [DbKeyColumn(false)]
        public int IdLanguage { get; set; }
        
        public override bool Equals(object obj)
        {
            CardInCollectionCount cicc = obj as CardInCollectionCount;
            if (null == cicc)
                return false;

            return cicc.IdCollection == IdCollection && cicc.IdGatherer == IdGatherer && cicc.IdLanguage == IdLanguage;
        }

        //There are not readonly because of reflection feeding by they never change after instance creation
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return IdCollection * 23 + IdGatherer;
        }
    }
}
