namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface ICardAllDbInfo
    {
        ICard Card { get; }
        IRarity Rarity { get; }
        IEdition Edition { get; }
        string IdScryFall { get; }
        IEnumerable<ICardInCollectionCount> Statistics { get; }
        ICollection<IPrice> Prices { get; }
        ICollection<string> VariationIdScryFalls { get; }
    }
}
