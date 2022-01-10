namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Windows.Input;

    using Common.ViewModel;

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
            RemoveCommand = new RelayCommand(RemoveCommandExecute);
            AddLinkedProperty(nameof(ChangedCount), new [] { nameof(Count), nameof(CountLabel) });
        }
        
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
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
                if (_changedCount!= value)
                {
                    _changedCount = value;
                    OnNotifyPropertyChanged(nameof(ChangedCount));
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
        private void AddCommandExecute(object obj)
        {
            ChangedCount++;
        }
        public void Reset()
        {
            ChangedCount = 0;
        }

        internal CardType GetCardType()
        {
            return MultiPartCardManager.Instance.GetCardType(Card.CardAllDbInfo);
        }
        internal ShardColor GetColor()
        {
            return MultiPartCardManager.Instance.GetColor(Card.CardAllDbInfo);
        }
    }
}
