namespace MagicPictureSetDownloader.Interface
{
    public interface ICard : IIdName
    {
        string Text { get; }
        string Power { get; }
        string Toughness { get; }
        string CastingCost { get; }
        int? Loyalty { get; }
        string Type { get; }
        string PartName { get; }
        string OtherPartName { get; }

        bool IsMultiPart { get; }
        bool IsReverseSide { get; }
        bool IsSplitted { get; }
    }
}
