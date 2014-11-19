namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Translate : ITranslate
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
