namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System;
    using System.Windows.Input;
    using Common.ViewModel;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public class CardCollectionInputGraphicViewModel : NotifyPropertyChangedBase, IComparable<CardCollectionInputGraphicViewModel>
    {
        private readonly int _cardInCollection;
        private readonly ILanguage _language;
        private int _changedCount;

        public CardCollectionInputGraphicViewModel(CardViewModel card, ILanguage language, string nameInLanguage, int cardInCollection)
        {
            Card = card;
            NameInLanguage = nameInLanguage;
            _cardInCollection = cardInCollection;
            _language = language;
            AddCommand = new RelayCommand(AddCommandExecute);
            RemoveCommand = new RelayCommand(RemoveCommandExecute);
            AddLinkedProperty(nameof(ChangedCount), new [] { nameof(Count), nameof(CountLabel) });
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public CardViewModel Card { get; }
        public string NameInLanguage { get; }

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
        public int CompareTo(CardCollectionInputGraphicViewModel other)
        {
            return NameInLanguage.CompareTo(other.NameInLanguage);
        }
        public void Reset()
        {
            ChangedCount = 0;
        }

    }
}
