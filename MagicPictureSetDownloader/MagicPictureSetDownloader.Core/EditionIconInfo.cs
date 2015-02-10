namespace MagicPictureSetDownloader.Core
{
    internal class EditionIconInfo : IEditionIconInfo
    {
        public EditionIconInfo(string name, string url, string code)
        {
            Url = url;
            Code = code;
            Name = name;
        }
        public string Name { get; private set; }
        public string Url { get; set; }
        public byte[] Icon { get; set; }
        public string Code { get; private set; }
    }
}
