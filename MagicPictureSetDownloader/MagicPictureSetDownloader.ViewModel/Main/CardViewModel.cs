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
        private readonly ICardFace _currentFace;

        public CardViewModel(ICardAllDbInfo cardAllDbInfo, bool otherPart =  false)
        {
            CardAllDbInfo = cardAllDbInfo;
            Card = cardAllDbInfo.Card;
            _currentFace = otherPart ? Card.OtherCardFace : Card.MainCardFace;
            IEdition edition = cardAllDbInfo.Edition;
            Statistics = cardAllDbInfo.Statistics.Select(s => new StatisticViewModel(s)).ToArray();
            Prices = cardAllDbInfo.Prices.Select(p => new PriceViewModel(p,edition)).ToArray();
            IsDownSide = false;
            Edition = edition;
            Rarity = cardAllDbInfo.Rarity;
            IdScryFall = cardAllDbInfo.IdScryFall;
            VariationIdScryFalls = cardAllDbInfo.VariationIdScryFalls.ToArray();
            IsMultiPart = MultiPartCardManager.Instance.HasMultiPart(Card);
            Is90DegreeSide = MultiPartCardManager.Instance.Is90DegreeSide(Card) || MultiPartCardManager.Instance.Is90DegreeFrontSide(Card);
            if (!string.IsNullOrWhiteSpace(_currentFace.Power) && !string.IsNullOrWhiteSpace(_currentFace.Toughness))
            {
                PowerToughnessLoyaltyDefense = string.Format("{0}/{1}", _currentFace.Power, _currentFace.Toughness);
                PowerToughnessLoyaltyDefenseText = "Power/Toughness";
            }
            else if (!string.IsNullOrWhiteSpace(_currentFace.Loyalty))
            {
                PowerToughnessLoyaltyDefense = _currentFace.Loyalty;
                PowerToughnessLoyaltyDefenseText = "Loyalty";
            }
            else if (!string.IsNullOrWhiteSpace(_currentFace.Defense))
            {
                PowerToughnessLoyaltyDefense = _currentFace.Defense;
                PowerToughnessLoyaltyDefenseText = "Defense";
            }

            if (!string.IsNullOrWhiteSpace(CastingCost))
            {
                DisplayedCastingCost = CastingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            
            if (IsMultiPart && !otherPart)
            {
                OtherCardPart = new CardViewModel(cardAllDbInfo, true);
                if (MultiPartCardManager.Instance.IsDownSide(cardAllDbInfo.Card))
                {
                    OtherCardPart.IsDownSide = true;
                }
            }
        }

        public ICardAllDbInfo CardAllDbInfo { get; }
        public IEdition Edition { get; }
        public IRarity Rarity { get; }
        public string IdScryFall { get; }
        
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
            get { return _currentFace.Type; }
        }
        public string CastingCost
        {
            get { return _currentFace.CastingCost; }
        }
        public string AllPartCastingCost
        {
            get { return IsMultiPart ? $"{CastingCost} {OtherCardPart.CastingCost}" : CastingCost; }
        }
        public string Text
        {
            get { return _currentFace.Text; }
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
        public string PowerToughnessLoyaltyDefense { get; }
        public string PowerToughnessLoyaltyDefenseText { get; }
        public string[] DisplayedCastingCost { get; }
        public string[] VariationIdScryFalls { get; }
        internal ICard Card { get; }
    }
}
