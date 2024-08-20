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
        private readonly ICardFace[] _cardFaces;

        public CardViewModel(ICardAllDbInfo cardAllDbInfo)
        {
            CardAllDbInfo = cardAllDbInfo;
            Card = cardAllDbInfo.Card;
            _cardFaces = cardAllDbInfo.CardFaces.ToArray(); ;
            MainCardFace = cardAllDbInfo.MainCardFace;
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
            if (!string.IsNullOrWhiteSpace(MainCardFace.Power) && !string.IsNullOrWhiteSpace(MainCardFace.Toughness))
            {
                PowerToughnessLoyaltyDefense = string.Format("{0}/{1}", MainCardFace.Power, MainCardFace.Toughness);
                PowerToughnessLoyaltyDefenseText = "Power/Toughness";
            }
            else if (!string.IsNullOrWhiteSpace(MainCardFace.Loyalty))
            {
                PowerToughnessLoyaltyDefense = MainCardFace.Loyalty;
                PowerToughnessLoyaltyDefenseText = "Loyalty";
            }
            else if (!string.IsNullOrWhiteSpace(MainCardFace.Defense))
            {
                PowerToughnessLoyaltyDefense = MainCardFace.Defense;
                PowerToughnessLoyaltyDefenseText = "Defense";
            }

            if (!string.IsNullOrWhiteSpace(CastingCost))
            {
                DisplayedCastingCost = CastingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            //ALERT TO BE REVIEW IsMultiPart
            /*
            if (IsMultiPart && !otherPart)
            {
                OtherCardPart = new CardViewModel(cardAllDbInfo, true);
                if (MultiPartCardManager.Instance.IsDownSide(cardAllDbInfo.CardPart2))
                {
                    OtherCardPart.IsDownSide = true;
                }
            }
            */
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
            get { return MainCardFace.Type; }
        }
        public string CastingCost
        {
            get { return MainCardFace.CastingCost; }
        }
        public string AllPartCastingCost
        {
            get { return IsMultiPart ? $"{CastingCost} {OtherCardPart.CastingCost}" : CastingCost; }
        }
        public string Text
        {
            get { return MainCardFace.Text; }
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
        internal ICardFace MainCardFace { get; }
    }
}
