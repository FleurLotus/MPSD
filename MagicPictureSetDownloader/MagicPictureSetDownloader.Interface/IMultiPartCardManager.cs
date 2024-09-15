namespace MagicPictureSetDownloader.Interface
{
    public interface IMultiPartCardManager
    {
        bool HasMultiPart(ICard card);
        bool IsDownSide(ICard card);
        bool Is90DegreeBackSide(ICard card);
        bool Is90DegreeFrontSide(ICard card);
        bool IsSpecial(ICard card);
        ShardColor GetColor(ICard card);
        CardType GetCardType(ICard card);
        CardSubType GetCardSubType(ICard card);
    }
}
