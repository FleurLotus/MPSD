using MagicPictureSetDownloader.ScryFall.JsonLite;

namespace MagicPictureSetDownloader.ScryFall
{
    public static class Tranformation
    {
        public static bool SetToIgnore(Set s)
        {
            return s.SetType switch
            {
                SetType.Token => true,
                SetType.Memorabilia => true,
                _ => false,
            };

        }
    }
}
