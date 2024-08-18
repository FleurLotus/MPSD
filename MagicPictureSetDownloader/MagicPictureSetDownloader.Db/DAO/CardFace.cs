namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;

    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class CardFace : ICardFace
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Text { get; set; }
        [DbColumn]
        public string Power { get; set; }
        [DbColumn]
        public string Toughness { get; set; }
        [DbColumn]
        public string CastingCost { get; set; }
        [DbColumn]
        public string Loyalty { get; set; }
        [DbColumn]
        public string Defense { get; set; }
        [DbColumn]
        public string Type { get; set; }
        [DbColumn]
        public string Url { get; set; }
    }
}
