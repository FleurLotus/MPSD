namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IBatch : IDisposable
    {
        void Commit();
    }
}