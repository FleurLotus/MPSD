namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;

    [DbTable]
    internal class ExternalIds
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string IdScryFall { get; set; }
        [DbColumn]
        public string CardIdSource { get; set; }
        [DbColumn]
        public string ExternalId { get; set; }
    }
}
