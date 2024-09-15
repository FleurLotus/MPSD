namespace MagicPictureSetDownloader.Core
{
    internal class CardFaceWithExtraInfo
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string CastingCost { get; set; }
        public string Loyalty { get; set; }
        public string Defense { get; set; }
        public string Type { get; set; }
        public string PictureUrl { get; set; }
        public bool IsMainFace { get; set; }
    }
}
