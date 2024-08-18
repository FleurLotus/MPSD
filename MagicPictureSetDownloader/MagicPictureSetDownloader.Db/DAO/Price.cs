namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Price : IPrice
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public DateTime AddDate { get; set; }
        [DbColumn]
        public string Source { get; set; }
        [DbColumn]
        public string IdScryFall { get; set; }
        [DbColumn]
        public bool Foil { get; set; }
        [DbColumn]
        public int Value { get; set; }
    }
}
