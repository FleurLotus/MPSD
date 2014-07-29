namespace MagicPictureSetDownloader.ViewModel
{
    using Common.ViewModel;
    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;
    using MagicPictureSetDownloader.Interface;

    class CardViewModel: NotifyPropertyChangedBase, ICardInfo
    {
        public CardViewModel(ICardAllDbInfo cardAllDbInfo)
        {
            Name = cardAllDbInfo.Card.Name;
            Edition = cardAllDbInfo.Edition.Name;
            BlockName = cardAllDbInfo.Edition.BlockName;
            Rarity = cardAllDbInfo.Rarity.Name;
            Type = cardAllDbInfo.Card.Type;
            CastingCost = cardAllDbInfo.Card.CastingCost;
        }

        public string Name { get; private set; }
        public string BlockName { get; private set; }
        public string Edition { get; private set; }
        public string Rarity { get; private set; }
        public string Type { get; private set; }
        public string CastingCost { get; private set; }
    }
}
