namespace MagicPictureSetDownloader.Db
{
    using MagicPictureSetDownloader.Interface;

    public static class CardCountKeys
    {
        public static readonly ICardCountKey Standard = new CardCountKey(false);
        public static readonly ICardCountKey Foil = new CardCountKey(true);
    }
}