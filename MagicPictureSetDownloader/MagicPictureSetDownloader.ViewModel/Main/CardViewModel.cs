namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Globalization;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;
    using MagicPictureSetDownloader.Interface;

    public class CardViewModel: NotifyPropertyChangedBase, ICardInfo
    {
        public CardViewModel(ICardAllDbInfo cardAllDbInfo)
            : this(cardAllDbInfo, false)
        {
        }

        private CardViewModel(ICardAllDbInfo cardAllDbInfo, bool otherPart)
        {
            ICard card = otherPart ? cardAllDbInfo.CardPart2 : cardAllDbInfo.Card;
            IEdition edition = cardAllDbInfo.Edition;
            
            Name = card.Name;
            IsDownSide = false;
            PartName = card.PartName;
            Edition = edition.Name;
            BlockName = edition.BlockName;
            Rarity = cardAllDbInfo.Rarity.Name;
            Type = card.Type;
            CastingCost = card.CastingCost;
            IsMultiPart = card.IsMultiPart;
            IdGatherer = otherPart ? cardAllDbInfo.IdGathererPart2 : cardAllDbInfo.IdGatherer;

            Text = card.Text;
            if (!string.IsNullOrWhiteSpace(card.Power) && !string.IsNullOrWhiteSpace(card.Toughness))
            {
                PowerToughnessLoyalty = string.Format("{0}/{1}", card.Power, card.Toughness);
                PowerToughnessLoyaltyText= "Power/Toughness";
            }
            else if (card.Loyalty.HasValue)
            {
                PowerToughnessLoyalty = card.Loyalty.Value.ToString(CultureInfo.InvariantCulture);
                PowerToughnessLoyaltyText= "Loyalty";
            }

            if (!string.IsNullOrWhiteSpace(CastingCost))
                DisplayedCastingCost = CastingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (IsMultiPart && !otherPart)
            {
                OtherCardPart = new CardViewModel(cardAllDbInfo, true);
                if (!cardAllDbInfo.CardPart2.IsSplitted && !cardAllDbInfo.CardPart2.IsReverseSide)
                    OtherCardPart.IsDownSide = true;
            }
        }

        public string Name { get; private set; }
        public string BlockName { get; private set; }
        public string Edition { get; private set; }
        public string Rarity { get; private set; }
        public string Type { get; private set; }
        public string CastingCost { get; private set; }
        public int IdGatherer { get; private set; }
        
        public bool IsMultiPart { get; private set; }
        public bool IsDownSide { get; private set; }
        public string PartName { get; private set; }
        public CardViewModel OtherCardPart { get; private set; }

        public string Text { get; private set; }
        public string PowerToughnessLoyalty { get; private set; }
        public string PowerToughnessLoyaltyText { get; private set; }
        public string[] DisplayedCastingCost { get; private set; }
    }
}
