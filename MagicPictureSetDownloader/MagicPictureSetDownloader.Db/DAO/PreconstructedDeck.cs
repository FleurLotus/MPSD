namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Diagnostics;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class PreconstructedDeck : IPreconstructedDeck
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public int IdEdition { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Url { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
