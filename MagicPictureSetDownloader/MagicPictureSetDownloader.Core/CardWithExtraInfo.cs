namespace MagicPictureSetDownloader.Core
{
    internal class CardWithExtraInfo
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string CastingCost { get; set; }
        public int? Loyalty { get; set; }
        public string Type { get; set; }
        public string PictureUrl { get; set; }
        public string Rarity { get; set; }
    }
}
