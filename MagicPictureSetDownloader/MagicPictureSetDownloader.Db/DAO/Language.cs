namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;

    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Language : ILanguage
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string AlternativeName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
