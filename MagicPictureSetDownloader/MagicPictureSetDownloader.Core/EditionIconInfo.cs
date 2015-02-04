namespace MagicPictureSetDownloader.Core
{
    internal class EditionIconInfo
    {
        public EditionIconInfo(string name, string urlPage)
        {
            UrlPage = urlPage;
            Name = name;
        }
        public string Name { get; private set; }
        public string CorrectedName { get;  set; }
        public string UrlPage { get; private set; }
    }
}
