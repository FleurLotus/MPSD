
namespace MagicPictureSetDownloader.Core
{
    public class PictureInfo
    {
        public string Url { get; private set; }
        public string PictureUrl { get; private set; }

        public PictureInfo(string url, string pictureurl)
        {
            PictureUrl = pictureurl;
            Url = url;
        }
    }
}
