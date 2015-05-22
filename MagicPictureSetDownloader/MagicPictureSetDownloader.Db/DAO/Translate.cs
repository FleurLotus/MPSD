namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;

    [DbTable]
    internal class Translate
    {
        [DbColumn]
        [DbKeyColumn(false)]
        public int IdLanguage { get; set; }
        [DbColumn]
        [DbKeyColumn(false)]
        public int IdCard { get; set; }
        [DbColumn]
        public string Name { get; set; }
    }
}
