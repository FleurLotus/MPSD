namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    public interface ICardInfo
    {
        string Name { get; }
        string Edition { get; }
        string BlockName { get; }
        string Color { get; }
        string Type { get; }
    }
}
