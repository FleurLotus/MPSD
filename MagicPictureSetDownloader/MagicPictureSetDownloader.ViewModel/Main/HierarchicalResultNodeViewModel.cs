namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;

    public class HierarchicalResultNodeViewModel : HierarchicalResultViewModel
    {
        public HierarchicalResultNodeViewModel(IComparable value, CardViewModel card)
            : base(value)
        {
            Card = card;
        }
        public CardViewModel Card { get; private set; }
    }
}
