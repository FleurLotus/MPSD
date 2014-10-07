namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using MagicPictureSetDownloader.Interface;

    public interface ICardInfo
    {
        IEdition Edition { get; }
        string BlockName { get; }
        string Rarity { get; }
        string Name { get; }
        string Type { get; }
        string CastingCost { get; }
        int IdGatherer { get; }
       
    }
}
