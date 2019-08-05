namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IRarity: IIdName, IComparable
    {
        string Code { get; }
    }
}