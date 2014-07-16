using MagicPictureSetDownloader.Db;

namespace MagicPictureSetDownloader.Core
{
    internal class CardWithExtraInfo
    {
        public Card Card { get; set; }
        public string PictureUrl { get; set; }
        public string Rarity { get; set; }
    }
}
