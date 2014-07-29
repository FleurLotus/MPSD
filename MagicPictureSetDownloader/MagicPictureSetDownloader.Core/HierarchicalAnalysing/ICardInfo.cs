namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    public interface ICardInfo
    {
        string Edition { get; }
        string BlockName { get; }
        string Rarity { get; }
        string Name { get; }
        string Type { get; }
        string CastingCost { get; }
        int IdGatherer { get; }
    }
}
