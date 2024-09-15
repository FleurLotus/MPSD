namespace MagicPictureSetDownloader.Interface
{
    public interface ICardFace : IIdName
    {
        string Text { get; }
        string Power { get; }
        string Toughness { get; }
        string CastingCost { get; }
        string Loyalty { get; }
        string Defense { get; }
        string Type { get; }
        int IdCard { get; }
        bool IsMainFace { get; }
    }
}
