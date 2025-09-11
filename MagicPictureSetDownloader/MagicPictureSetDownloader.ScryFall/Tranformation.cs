namespace MagicPictureSetDownloader.ScryFall
{
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public static class Tranformation
    {
        public static bool CardToIgnore(Card s)
        {
            return s.Layout switch
            {
                Layout.ArtSeries => true,
                Layout.DoubleFacedToken => true,
                Layout.Emblem => true,
                Layout.Token => true,
                _ => false,
            };

        }
    }
}