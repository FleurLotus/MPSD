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
        IEnumerable<ICardInCollectionCount> Statistics { get; }
        ICollection<IPrice> Prices { get; }
        ICollection<int> VariationIdGatherers { get; }
    }
}
