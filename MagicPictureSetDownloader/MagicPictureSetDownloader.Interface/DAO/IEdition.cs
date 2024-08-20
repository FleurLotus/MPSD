namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IEdition : IIdName, IComparable
    {
        string Code { get; }
        int? IdBlock { get; }
        string BlockName { get; }
        DateTime? ReleaseDate { get; }
        int? CardNumber { get; }
        bool IsCode(string code);
        string AlternativeCode(ExportFormat format);
        bool HasFoil { get; }
    }
}