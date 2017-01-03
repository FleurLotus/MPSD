namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Ruling : IRuling
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public DateTime AddDate { get; set; }
        [DbColumn]
        public int IdCard { get; set; }
        [DbColumn]
        public string Text { get; set; }
    }
}
