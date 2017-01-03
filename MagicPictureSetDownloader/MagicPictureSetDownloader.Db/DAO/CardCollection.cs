namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable(Name = "Collection")]
    internal class CardCollection : ICardCollection
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
