namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Windows.Input;

    using Common.ViewModel;
    using Common.ViewModel.Command;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public partial class CardCollectionInputGraphicViewModel : NotifyPropertyChangedBase
    {
        private int _cardInCollection;
        private int _changedCount;

        public CardCollectionInputGraphicViewModel(CardViewModel card)
        {
            Card = card;
            AddCommand = new RelayCommand(AddCommandExecute);
            AddFourCommand = new RelayCommand(AddFourCommandExecute);
            RemoveCommand = new RelayCommand(RemoveCommandExecute);
            RemoveFourCommand = new RelayCommand(RemoveFourCommandExecute);
            AddLinkedProperty(nameof(ChangedCount), new[] { nameof(Count), nameof(CountLabel) });
        }

        public ICommand AddCommand { get; }
        public ICommand AddFourCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand RemoveFourCommand { get; }
        public CardViewModel Card { get; }
        public string NameInLanguage { get; private set; }
        public string Name { get { return Card.Name; } }

        public int Count
        {
            get { return _cardInCollection + ChangedCount; }
        }
        public string CountLabel
        {
            get
            {
                if (ChangedCount == 0)
                {
                    return _cardInCollection.ToString();
                }
                return string.Format("{0} {1:+0;-0}", _cardInCollection, ChangedCount);
            }
        }
        public int ChangedCount
        {
            get
            {
                return _changedCount;
            }
            private set
            {
                if (_changedCount != value)
                {
                    _changedCount = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public void SetInfo(string nameInLanguage, int cardInCollection)
        {
            NameInLanguage = nameInLanguage;
            _cardInCollection = cardInCollection;
            ChangedCount = 0;
        }

        private void RemoveCommandExecute(object obj)
        {
            if (ChangedCount + _cardInCollection > 0)
            {
                ChangedCount--;
            }
        }
        private void RemoveFourCommandExecute(object obj)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ChangedCount + _cardInCollection > 0)
                {
                    ChangedCount--;
                }
            }
        }

        private void AddCommandExecute(object obj)
        {
            ChangedCount++;
        }
        private void AddFourCommandExecute(object obj)
        {
            ChangedCount += 3;
        }
        public void Reset()
        {
            ChangedCount = 0;
        }
        internal IRarity GetCardRarity()
        {
            return Card.CardAllDbInfo.Rarity;
        }
        internal CardType GetCardType()
        {
            return MultiPartCardManager.Instance.GetCardType(Card.CardAllDbInfo.Card);
        }
        internal ShardColor GetColor()
        {
            return MultiPartCardManager.Instance.GetColor(Card.CardAllDbInfo.Card);
        }
    }
}