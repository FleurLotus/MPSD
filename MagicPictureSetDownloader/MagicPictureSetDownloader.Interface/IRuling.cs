namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IRuling
    {
        int Id { get; }
        DateTime AddDate { get; }
        int IdCard { get; }
        string Text { get; }
    }
}
