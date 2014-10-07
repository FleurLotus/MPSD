namespace MagicPictureSetDownloader.Db.DAO
{
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Card : ICard
    {
        [DbColumn]
        [DbKeyColumn(true)]
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
        public int? Loyalty { get; set; }
        [DbColumn]
        public string Type { get; set; }
        [DbColumn]
        public string PartName { get; set; }
        [DbColumn]
        public string OtherPartName { get; set; }

        public bool IsMultiPart
        {
            get { return OtherPartName != null; }
        }
        public bool IsReverseSide
        {
            get { return IsMultiPart && CastingCost == null; }
        }
        public bool IsSplitted
        {
            get { return IsMultiPart && PartName != Name && OtherPartName != Name; }
        }
        public override string ToString()
        {
            return IsSplitted ? Name + PartName : Name;
        }
    }
}
