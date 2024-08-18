namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;

    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class CardCardFace : ICardCardFace
    {
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdCard { get; set; }
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdCardFace { get; set; }
    }
}
