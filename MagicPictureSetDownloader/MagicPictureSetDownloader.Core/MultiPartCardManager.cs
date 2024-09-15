namespace MagicPictureSetDownloader.Core
{
    using System;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall;

    public class MultiPartCardManager: IMultiPartCardManager
    {
        private static readonly Lazy<IMultiPartCardManager> _lazy = new Lazy<IMultiPartCardManager>(() => new MultiPartCardManager());

        public static IMultiPartCardManager Instance
        {
            get { return _lazy.Value; }
        }

        private MultiPartCardManager()
        {
        }

        public bool HasMultiPart(ICard card)
        {
            return card.OtherCardFace != null;
        }
        // Up/Down
        public bool IsDownSide(ICard card)
        {
            return card.Layout == Layout.Flip.ToString();
        }

        private bool IsSplitted(ICard card)
        {
            return card.Layout == Layout.Split.ToString(); 
        }

        //Aftermath
        public bool Is90DegreeBackSide(ICard card)
        {
            return IsSplitted(card) && card.OtherCardFace != null && card.OtherCardFace.Text.StartsWith("Aftermath");
        }

        //Battle
        public bool Is90DegreeFrontSide(ICard card)
        {
            return (HasMultiPart(card) && MagicRules.IsBattle(card.MainCardFace.Type)) || (IsSplitted(card) && !Is90DegreeBackSide(card));
        }

        public bool IsSpecial(ICard card)
        {
            return MagicRules.IsSpecial(card.MainCardFace.Type);
        }

        public ShardColor GetColor(ICard card)
        {
            ShardColor color = MagicRules.GetColor(card.MainCardFace.CastingCost);
            if (IsSplitted(card))
            {
                color |= MagicRules.GetColor(card.OtherCardFace.CastingCost);
            }

            return color;
        }

        public CardType GetCardType(ICard card)
        {
            CardType type = MagicRules.GetCardType(card.MainCardFace.Type, card.MainCardFace.CastingCost);
            if (IsSplitted(card))
            {
                type |= MagicRules.GetCardType(card.OtherCardFace.Type, card.OtherCardFace.CastingCost);
            }
            return type;
        }

        public CardSubType GetCardSubType(ICard card)
        {
            CardSubType subType = MagicRules.GetCardSubType(card.MainCardFace.Type);
            if (IsSplitted(card))
            {
                subType |= MagicRules.GetCardSubType(card.MainCardFace.Type);
            }

            return subType;

        }
    }
}
