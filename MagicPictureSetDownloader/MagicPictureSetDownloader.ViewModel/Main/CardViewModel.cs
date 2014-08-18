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
        {
            ICard card = cardAllDbInfo.Card;
            IEdition edition = cardAllDbInfo.Edition;
            
            Name = card.Name;
            Edition = edition.Name;
            BlockName = edition.BlockName;
            Rarity = cardAllDbInfo.Rarity.Name;
            Type = card.Type;
            CastingCost = card.CastingCost;
            IdGatherer = cardAllDbInfo.IdGatherer;

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
        }

        public string Name { get; private set; }
        public string BlockName { get; private set; }
        public string Edition { get; private set; }
        public string Rarity { get; private set; }
        public string Type { get; private set; }
        public string CastingCost { get; private set; }
        public int IdGatherer { get; private set; }


        public string Text { get; private set; }
        public string PowerToughnessLoyalty { get; private set; }
        public string PowerToughnessLoyaltyText { get; private set; }
        public string[] DisplayedCastingCost { get; private set; }
    }
}
