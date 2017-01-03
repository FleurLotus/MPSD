namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;

    [DbTable]
    internal class Translate
    {
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdLanguage { get; set; }
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdCard { get; set; }
        [DbColumn]
        public string Name { get; set; }
    }
}
