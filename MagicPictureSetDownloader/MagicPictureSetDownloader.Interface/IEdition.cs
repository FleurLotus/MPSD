namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IEdition : IIdName
    {
        string Code { get; }
        int? IdBlock { get; }
        string BlockName { get; }
        int? BlockPosition { get; }
        string GathererName { get; }
        DateTime? ReleaseDate { get; }
        int? CardNumber { get; }
        bool Completed { get; }
        bool IsCode(string code);
        string AlternativeCode(ExportFormat format);
    }
}