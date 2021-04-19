namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class BackSideModalDoubleFacedCard : IBackSideModalDoubleFacedCard
    {
        [DbColumn]
        public string Name { get; set; }
    }
}
