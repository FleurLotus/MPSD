﻿namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface IMultiPartCardManager
    {
        bool HasMultiPart(ICard card);
        bool IsDownSide(ICard card);
        bool Is90DegreeSide(ICard card);

        bool ShouldIgnore(ICard card);
        bool IsSecondPartOfSplitted(ICard card);
        ICard GetOtherPartCard(ICard card, Func<string, string, ICard> getCard);
        ShardColor GetColor(ICardAllDbInfo cai);
        CardType GetCardType(ICardAllDbInfo cai);
        CardSubType GetCardSubType(ICardAllDbInfo cai);
    }
}
