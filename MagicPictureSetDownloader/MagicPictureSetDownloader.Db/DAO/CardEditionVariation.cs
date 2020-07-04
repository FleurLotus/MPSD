namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class CardEditionVariation : ICardEditionVariation
    {
        [DbColumn]
        public int IdGatherer { get; set; }
        [DbColumn]
        public int OtherIdGatherer { get; set; }
        [DbColumn]
        public string Url { get; set; }
    }
}
