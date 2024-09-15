namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class PreconstructedDeckCardEdition : IPreconstructedDeckCardEdition
    {
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public int IdPreconstructedDeck { get; set; }
        [DbColumn(Kind = ColumnKind.PrimaryKey)]
        public string IdScryFall { get; set; }
        [DbColumn]
        public int Number { get; set; }
    }
}
