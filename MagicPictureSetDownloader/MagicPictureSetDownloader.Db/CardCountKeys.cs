namespace MagicPictureSetDownloader.Interface
{
    public static class CardCountKeys
    {
        public static readonly ICardCountKey Standard = new CardCountKey(false, false);
        public static readonly ICardCountKey Foil = new CardCountKey(true, false);
        public static readonly ICardCountKey AltArt = new CardCountKey(false, true);
        public static readonly ICardCountKey FoilAltArt = new CardCountKey(true, true);

        public static ICardCountKey[] AllKeys = new ICardCountKey[] { Standard, Foil, AltArt, FoilAltArt };
    }
}
