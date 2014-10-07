namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface ICardAllDbInfo
    {
        ICard Card { get; }
        IRarity Rarity { get; }
        IEdition Edition { get; }
        int IdGatherer { get; }
        ICard CardPart2 { get; }
        int IdGathererPart2 { get; }
        ICollection<ICardInCollectionCount> Statistics { get; }
    }
}
