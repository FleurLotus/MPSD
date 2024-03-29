﻿namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using MagicPictureSetDownloader.Interface;
    
    public class MultiPartCardManager: IMultiPartCardManager
    {
        private static readonly Lazy<IMultiPartCardManager> _lazy = new Lazy<IMultiPartCardManager>(() => new MultiPartCardManager());
        private readonly HashSet<string> _backSideModalDoubleFacedCards = new HashSet<string>();

        public static IMultiPartCardManager Instance
        {
            get { return _lazy.Value; }
        }

        private MultiPartCardManager()
        {
        }

        public void ClearBackSideModalDoubleFacedCards()
        {
            _backSideModalDoubleFacedCards.Clear();
        }
        public void AddBackSideModalDoubleFacedCard(string backSideModalDoubleFacedCard)
        {
            _backSideModalDoubleFacedCards.Add(backSideModalDoubleFacedCard);
        }

        public bool HasMultiPart(ICard card)
        {
            return card.OtherPartName != null;
        }

        public bool IsDownSide(ICard card)
        {
            return HasMultiPart(card) &&
                  !IsSplitted(card) && !IsReverseSide(card) && !IsBackSideOfModalDoubleFacedCard(card) && !IsMultiCard(card) && !IsSameDisplay(card);
        }

        //Adventure
        private bool IsSameDisplay(ICard card)
        {
             return HasMultiPart(card) && card.Type.EndsWith("Adventure"); 
        }

        //MDFC
        private bool IsBackSideOfModalDoubleFacedCard(ICard card)
        {
            //There is no way to know the recto from verso for MDFC so need to keep the referential manually
            return _backSideModalDoubleFacedCards.Contains(card.Name);
        }

        //Recto-Verso
        private bool IsReverseSide(ICard card)
        {
            //Exclude land because of Westvale Abbey and Hanweir Battlements
            //But keep transformed to land
            return HasMultiPart(card) && card.CastingCost == null &&
              (!card.Type.ToLowerInvariant().Contains("land") || (card.Text != null && card.Text.StartsWith("(Transforms from")));
        }
        //Multiple part on the same side (it is not the case of Up/Down)
        private bool IsSplitted(ICard card)
        {
            return HasMultiPart(card) && card.PartName != card.Name && card.OtherPartName != card.Name; 
        }

        //B.F.M. (Big Furry Monster)
        private bool IsMultiCard(ICard card)
        {
            return HasMultiPart(card) && card.PartName == card.Name && card.OtherPartName == card.Name && card.CastingCost != null;
        }

        //Aftermath
        public bool Is90DegreeSide(ICard card)
        {
            return IsSplitted(card) && card.Text != null && card.Text.StartsWith("Aftermath");
        }

        //Battle
        public bool Is90DegreeFrontSide(ICard card)
        {
            return HasMultiPart(card) && MagicRules.IsBattle(card.Type);
        }

        public bool ShouldIgnore(ICard card)
        {
            //Ignore the reverse side of a recto-verso card
            return IsReverseSide(card) || IsBackSideOfModalDoubleFacedCard(card);
        }

        public bool IsSecondPartOfSplitted(ICard card)
        {
            return IsSplitted(card) && !card.Name.StartsWith(card.PartName);
        }
        public bool IsSpecial(ICardAllDbInfo cai)
        {
            if (MagicRules.IsSpecial(cai.Card.Type))
            {
                return true;
            }
            return IsSplitted(cai.Card) && MagicRules.IsSpecial(cai.CardPart2.Type);
        }

        public ICard GetOtherPartCard(ICard card, Func<string, string, ICard> getCard)
        {
            if (!HasMultiPart(card))
            {
                return null;
            }

            return IsSplitted(card) ? getCard(card.Name, card.OtherPartName) : getCard(card.OtherPartName, null);
        }

        public ShardColor GetColor(ICardAllDbInfo cai)
        {
            ShardColor color = MagicRules.GetColor(cai.Card.CastingCost);
            if (IsSplitted(cai.Card))
            {
                color |= MagicRules.GetColor(cai.CardPart2.CastingCost);
            }

            return color;
        }

        public CardType GetCardType(ICardAllDbInfo cai)
        {
            CardType type = MagicRules.GetCardType(cai.Card.Type, cai.Card.CastingCost);
            if (IsSplitted(cai.Card))
            {
                type |= MagicRules.GetCardType(cai.CardPart2.Type, cai.CardPart2.CastingCost);
            }
            return type;
        }

        public CardSubType GetCardSubType(ICardAllDbInfo cai)
        {
            CardSubType subType = MagicRules.GetCardSubType(cai.Card.Type);
            if (IsSplitted(cai.Card))
            {
                subType |= MagicRules.GetCardSubType(cai.CardPart2.Type);
            }

            return subType;

        }
    }
}
