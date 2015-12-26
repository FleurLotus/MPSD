namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using MagicPictureSetDownloader.Interface;

    public interface ICardInfo
    {
        IEdition Edition { get; }
        string BlockName { get; }
        IRarity Rarity { get; }
        string Name { get; }
        string Type { get; }
        string CastingCost { get; }
        string AllPartCastingCost { get; }
        int IdGatherer { get; }
    }
}
