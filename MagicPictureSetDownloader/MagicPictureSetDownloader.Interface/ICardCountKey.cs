namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface ICardCountKey: IEquatable<ICardCountKey>
    {
        bool IsFoil { get; }
        bool IsAltArt { get; }
    }
}
