namespace MagicPictureSetDownloader.Core
{
    public class SetInfo
    {
        public SetInfo(string name, string baseSearchUrl)
        {
            Name = name;
            BaseSearchUrl = baseSearchUrl;
        }

        public string BaseSearchUrl { get; private set; }
        public string Name { get; private set; }
    }
}
