namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IAudit
    {
        int Id { get; }
        DateTime OperationDate { get; }
        int IdCollection { get; }
        int? IdGatherer { get; }
        bool? IsFoil { get; }
        bool? IsAltArt { get; }
        int? IdLanguage { get; }
        int Quantity { get; }
    }
}
