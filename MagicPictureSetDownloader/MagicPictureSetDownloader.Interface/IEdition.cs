namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IEdition
    {
        int Id { get; }
        string Name { get; }
        string Code { get; }
        int? IdBlock { get; }
        string BlockName { get; }
        int? BlockPosition { get; }
        string GathererName { get; }
        DateTime? ReleaseDate { get; }
        int? CardNumber { get; }
    }
}