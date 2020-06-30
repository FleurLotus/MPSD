namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable(Name = "CardEditionsInCollection")]
    internal class CardInCollectionCount : ICardInCollectionCount
    {
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdCollection { get; set; }
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdGatherer { get; set; }
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
            CardInCollectionCount cicc = obj as CardInCollectionCount;
            if (null == cicc)
            {
                return false;
            }

            return cicc.IdCollection == IdCollection && cicc.IdGatherer == IdGatherer && cicc.IdLanguage == IdLanguage;
        }

        //There are not readonly because of reflection feeding by they never change after instance creation
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return IdCollection * 23 + IdGatherer;
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
            CardCount cardCount = new CardCount();
            cardCount.Add(CardCountKeys.Standard, Number);
            cardCount.Add(CardCountKeys.Foil, FoilNumber);
            cardCount.Add(CardCountKeys.AltArt, AltArtNumber);
            cardCount.Add(CardCountKeys.FoilAltArt, FoilAltArtNumber);

            return cardCount;
        }

    }
}
