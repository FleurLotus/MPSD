

namespace MagicPictureSetDownloader.Core
{
    public class CardInfo
    {
        public CardInfo(string name, string url, int rarityId)
        {
            Name = name;
            Url = url;
            RarityId = rarityId;
        }

        public string Name { get; private set; }
        public int RarityId { get; private set; }
        public string Url { get; private set; }
    }
}
