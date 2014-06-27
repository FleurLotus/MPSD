

namespace MagicPictureSetDownloader.Core
{
    public class CardInfo
    {
        public CardInfo(string name, string url, string rarity)
        {
            Name = name;
            Url = url;
            Rarity = rarity;
        }

        public string Name { get; private set; }
        public string Rarity { get; private set; }
        public string Url { get; private set; }
    }
}
