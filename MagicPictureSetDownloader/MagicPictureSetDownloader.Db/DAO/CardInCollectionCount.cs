namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable(Name = "CardEditionsInCollection")]
    internal class CardInCollectionCount : ICardInCollectionCount
    {
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdCollection { get; set; }
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public string IdScryFall { get; set; }
        [DbColumn]
        public int Number { get; set; }
        [DbColumn]
        public int FoilNumber { get; set; }
        [DbColumn]
        public int AltArtNumber { get; set; }
        [DbColumn]
        public int FoilAltArtNumber { get; set; }
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdLanguage { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj is not CardInCollectionCount cicc)
            {
                return false;
            }

            return cicc.IdCollection == IdCollection && cicc.IdScryFall == IdScryFall && cicc.IdLanguage == IdLanguage;
        }

        //There are not readonly because of reflection feeding by they never change after instance creation
        public override int GetHashCode()
        {
            return IdCollection * 23 + IdScryFall.GetHashCode();
        }

        public int GetCount(ICardCountKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (CardCountKeys.Standard.Equals(key))
            {
                return Number;
            }
            if (CardCountKeys.Foil.Equals(key))
            {
                return FoilNumber;
            }
            if (CardCountKeys.AltArt.Equals(key))
            {
                return AltArtNumber;
            }
            if (CardCountKeys.FoilAltArt.Equals(key))
            {
                return FoilAltArtNumber;
            }

            throw new ArgumentException("Unmanaged type of key", nameof(key));
        }
        public ICardCount GetCardCount()
        {
            CardCount cardCount = new CardCount
            {
                { CardCountKeys.Standard, Number },
                { CardCountKeys.Foil, FoilNumber },
                { CardCountKeys.AltArt, AltArtNumber },
                { CardCountKeys.FoilAltArt, FoilAltArtNumber }
            };

            return cardCount;
        }

    }
}
