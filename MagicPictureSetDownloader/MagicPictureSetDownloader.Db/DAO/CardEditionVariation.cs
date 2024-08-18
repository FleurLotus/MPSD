namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class CardEditionVariation : ICardEditionVariation
    {
        [DbColumn]
        public string IdScryFall { get; set; }
        [DbColumn]
        public string OtherIdScryFall { get; set; }
        [DbColumn]
        public string Url { get; set; }
    }
}
