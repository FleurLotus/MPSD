namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;

    public class HierarchicalResultNodeViewModel : HierarchicalResultViewModel
    {
        private readonly List<CardViewModel> _allCards = new List<CardViewModel>();

        public HierarchicalResultNodeViewModel(IComparable value)
            : base(value)
        {
        }

        public void AddCard(CardViewModel card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            _allCards.Add(card);
            //If edition is not selected in filter, multiple version of the same card could appear, keeps the description and so the picture of the most recent one.
            if (Card == null || (Card.Edition.ReleaseDate < card.Edition.ReleaseDate))
            {
                Card = card;
            }
        }

        public CardViewModel Card { get; private set; }
        public CardViewModel[] AllCard
        {
            get { return _allCards.ToArray(); }
        }
    }
}
