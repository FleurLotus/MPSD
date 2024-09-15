namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IPrice
    {
        int Id { get; }
        DateTime AddDate { get; }
        string Source { get; set; }
        string IdScryFall { get; }
        bool Foil { get; set; }
        int Value { get; }
    }
}
