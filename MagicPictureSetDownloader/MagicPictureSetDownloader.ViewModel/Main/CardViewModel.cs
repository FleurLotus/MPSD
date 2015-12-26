namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Globalization;
    using System.Linq;

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
            Card = otherPart ? cardAllDbInfo.CardPart2 : cardAllDbInfo.Card;
            IEdition edition = cardAllDbInfo.Edition;
            Statistics = cardAllDbInfo.Statistics.Select(s => new StatisticViewModel(s)).ToArray();
            
            IsDownSide = false;
            Edition = edition;
            Rarity = cardAllDbInfo.Rarity;
            IdGatherer = otherPart ? cardAllDbInfo.IdGathererPart2 : cardAllDbInfo.IdGatherer;
            if (!string.IsNullOrWhiteSpace(Card.Power) && !string.IsNullOrWhiteSpace(Card.Toughness))
            {
                PowerToughnessLoyalty = string.Format("{0}/{1}", Card.Power, Card.Toughness);
                PowerToughnessLoyaltyText = "Power/Toughness";
            }
            else if (Card.Loyalty.HasValue)
            {
                PowerToughnessLoyalty = Card.Loyalty.Value.ToString(CultureInfo.InvariantCulture);
                PowerToughnessLoyaltyText = "Loyalty";
            }

            if (!string.IsNullOrWhiteSpace(CastingCost))
                DisplayedCastingCost = CastingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (IsMultiPart && !otherPart)
            {
                OtherCardPart = new CardViewModel(cardAllDbInfo, true);
                if (!cardAllDbInfo.CardPart2.IsSplitted && !cardAllDbInfo.CardPart2.IsReverseSide && !cardAllDbInfo.CardPart2.IsMultiCard)
                    OtherCardPart.IsDownSide = true;
            }
        }

        public IEdition Edition { get; private set; }
        public IRarity Rarity { get; private set; }
        public int IdGatherer { get; private set; }
        
        public string Name
        {
            get { return Card.Name; }
        }
        public string BlockName
        {
            get { return Edition.BlockName; }
        }
        public string Type
        {
            get { return Card.Type; }
        }
        public string CastingCost
        {
            get { return Card.CastingCost; }
        }
        public string AllPartCastingCost
        {
            get { return IsMultiPart ? CastingCost + " " + OtherCardPart.CastingCost : CastingCost; }
        }

        public bool IsMultiPart
        {
            get { return Card.IsMultiPart; }
        }
        public string PartName
        {
            get { return Card.PartName; }
        }
        public string Text
        {
            get { return Card.Text; }
        }
        public bool IsDownSide { get; private set; }
        public CardViewModel OtherCardPart { get; private set; }
        public StatisticViewModel[] Statistics { get; private set; }
        public string PowerToughnessLoyalty { get; private set; }
        public string PowerToughnessLoyaltyText { get; private set; }
        public string[] DisplayedCastingCost { get; private set; }
        public ICard Card { get; private set; }
    }
}
