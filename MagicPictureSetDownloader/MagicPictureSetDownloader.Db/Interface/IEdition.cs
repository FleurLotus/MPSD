namespace MagicPictureSetDownloader.Db
{
    public interface IEdition
    {
        int Id { get; }
        string Name { get; }
        string Code { get; }
        int? IdBlock { get; }
        string BlockName { get; }
        int? BlockPosition { get; }
        string GathererName { get; }
    }
}