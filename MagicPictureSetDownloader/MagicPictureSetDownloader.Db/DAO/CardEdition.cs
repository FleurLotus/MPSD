namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class CardEdition : ICardEdition
    {
        [DbColumn]
        public int IdEdition { get; set; }
        [DbColumn]
        public int IdCard { get; set; }
        [DbColumn]
        public int IdRarity { get; set; }
        [DbColumn]
        public string IdScryFall { get; set; }
    }
}
