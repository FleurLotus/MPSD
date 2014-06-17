using System.Diagnostics;

namespace MagicPictureSetDownloader.Core
{
    [DebuggerDisplay("Set = {Alias}")]
    public class SetInfo
    {
        public string Alias { get; private set; }
        public string Url { get; private set; }
        public string PictureUrl { get; private set; }

        public SetInfo(string alias, string pictureurl, string url)
        {
            Alias = alias;
            PictureUrl = pictureurl;
            Url = url;
        }
    }
}
