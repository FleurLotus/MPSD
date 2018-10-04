namespace MagicPictureSetDownloader.Core
{
    internal class EditionInfo
    {
        public EditionInfo(string name, string baseSearchUrl)
        {
            Name = name;
            BaseSearchUrl = baseSearchUrl;
        }

        public string BaseSearchUrl { get; }
        public string Name { get; }
    }
}
