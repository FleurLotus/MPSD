namespace MagicPictureSetDownloader.Interface
{
    public class CardCountKey : ICardCountKey
    {
        public CardCountKey(bool isFoil, bool isAltArt)
        {
            IsFoil = isFoil;
            IsAltArt = isAltArt;
        }

        public bool IsFoil { get; }
        public bool IsAltArt { get; }

        public override bool Equals(object o)
        {
            ICardCountKey other = o as ICardCountKey;
            if (other == null)
            {
                return false;
            }
            return Equals(other);
        }
        public override int GetHashCode()
        {
            int ret = 0;
            if (IsFoil)
            {
                ret += 1;
            }
            if (IsAltArt)
            {
                ret += 2;
            }
            return ret;
        }
        public override string ToString()
        {
            if (IsFoil)
            {
                return IsAltArt ? "FoilAltArt" : "Foil";
            }
            return IsAltArt ? "AltArt" : "Standard";
        }
        public bool Equals(ICardCountKey other)
        {
            if (other == null)
            {
                return false;
            }

            return IsFoil == other.IsFoil && IsAltArt == other.IsAltArt;
        }
    }
}
