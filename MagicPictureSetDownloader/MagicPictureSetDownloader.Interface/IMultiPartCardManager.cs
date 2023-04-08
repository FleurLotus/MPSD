namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IMultiPartCardManager
    {
        void AddBackSideModalDoubleFacedCard(string backSideModalDoubleFacedCard);
        void ClearBackSideModalDoubleFacedCards();
        bool HasMultiPart(ICard card);
        bool IsDownSide(ICard card);
        bool Is90DegreeSide(ICard card);
        bool Is90DegreeFrontSide(ICard card);
        bool ShouldIgnore(ICard card);
        bool IsSecondPartOfSplitted(ICard card);
        bool IsSpecial(ICardAllDbInfo cai);
        ICard GetOtherPartCard(ICard card, Func<string, string, ICard> getCard);
        ShardColor GetColor(ICardAllDbInfo cai);
        CardType GetCardType(ICardAllDbInfo cai);
        CardSubType GetCardSubType(ICardAllDbInfo cai);
    }
}
