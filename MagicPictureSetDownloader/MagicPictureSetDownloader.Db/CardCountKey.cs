namespace MagicPictureSetDownloader.Db
{

    using MagicPictureSetDownloader.Interface;

    public class CardCountKey : ICardCountKey
    {
        public CardCountKey(bool isFoil)
        {
            IsFoil = isFoil;
        }

        public bool IsFoil { get; }

        public override bool Equals(object o)
        {
            if (o is not ICardCountKey other)
            {
                return false;
            }
            return Equals(other);
        }
        public override int GetHashCode()
        {
            return IsFoil ? 1 : 0;
        }
        public override string ToString()
        {
            return IsFoil ? "Foil" : "Standard";
        }
        public bool Equals(ICardCountKey other)
        {
            if (other == null)
            {
                return false;
            }

            return IsFoil == other.IsFoil;
        }
    }
}