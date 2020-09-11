namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
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
            Prices = cardAllDbInfo.Prices.Select(p => new PriceViewModel(p,edition)).ToArray();
            IsDownSide = false;
            Edition = edition;
            Rarity = cardAllDbInfo.Rarity;
            IdGatherer = otherPart ? cardAllDbInfo.IdGathererPart2 : cardAllDbInfo.IdGatherer;
            VariationIdGatherers = cardAllDbInfo.VariationIdGatherers.ToArray();
            IsMultiPart = MultiPartCardManager.Instance.HasMultiPart(Card);
            Is90DegreeSide = MultiPartCardManager.Instance.Is90DegreeSide(Card);
            if (!string.IsNullOrWhiteSpace(Card.Power) && !string.IsNullOrWhiteSpace(Card.Toughness))
            {
                PowerToughnessLoyalty = string.Format("{0}/{1}", Card.Power, Card.Toughness);
                PowerToughnessLoyaltyText = "Power/Toughness";
            }
            else if (!string.IsNullOrWhiteSpace(Card.Loyalty))
            {
                PowerToughnessLoyalty = Card.Loyalty;
                PowerToughnessLoyaltyText = "Loyalty";
            }

            if (!string.IsNullOrWhiteSpace(CastingCost))
            {
                DisplayedCastingCost = CastingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (IsMultiPart && !otherPart)
            {
                OtherCardPart = new CardViewModel(cardAllDbInfo, true);
                if (MultiPartCardManager.Instance.IsDownSide(cardAllDbInfo.CardPart2))
                {
                    OtherCardPart.IsDownSide = true;
                }
            }
        }

        public IEdition Edition { get; }
        public IRarity Rarity { get; }
        public int IdGatherer { get; }
        
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
        public string PartName
        {
            get { return Card.PartName; }
        }
        public string Text
        {
            get { return Card.Text; }
        }
        public IRuling[] Rulings
        {
            get { return Card.Rulings; }
        }
        public string ToString(int? languageId)
        {
            return Card.ToString(languageId);
        }
        public bool IsMultiPart { get; }
        public bool Is90DegreeSide { get; }
        public bool IsDownSide { get; private set; }
        public CardViewModel OtherCardPart { get; }
        public StatisticViewModel[] Statistics { get; }
        public PriceViewModel[] Prices { get; }
        public string PowerToughnessLoyalty { get; }
        public string PowerToughnessLoyaltyText { get; }
        public string[] DisplayedCastingCost { get; }
        public int[] VariationIdGatherers { get; }
        internal ICard Card { get; }
    }
}
