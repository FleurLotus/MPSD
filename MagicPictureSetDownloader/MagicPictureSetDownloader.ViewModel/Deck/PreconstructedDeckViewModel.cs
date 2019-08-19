namespace MagicPictureSetDownloader.ViewModel.Deck
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Common.ViewModel;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public class PreconstructedDeckViewModel: NotifyPropertyChangedBase
    {
        private KeyValuePair<CardViewModel, int> _selectedItem;
        private readonly IEdition _edition;

        public PreconstructedDeckViewModel(IPreconstructedDeck preconstructedDeck, IEdition edition, IEnumerable<KeyValuePair<CardViewModel, int>> cards)
        {
            PreconstructedDeck = preconstructedDeck;
            Name = preconstructedDeck.Name;
            _edition = edition;
            IDictionary<CardViewModel, int> temp = new Dictionary<CardViewModel, int>();
            foreach (KeyValuePair<CardViewModel, int> kv in cards)
            {
                temp.Add(kv);
            }
            Composition = new ReadOnlyDictionary<CardViewModel, int>(temp);
        }

        public string Name { get; }
        public string Edition { get { return _edition.Name; } }
        public DateTime? EditionDate { get { return _edition.ReleaseDate; } }
        public IPreconstructedDeck PreconstructedDeck { get; }
        public IDictionary<CardViewModel, int> Composition { get; }

        public KeyValuePair<CardViewModel, int> SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (value.Key != _selectedItem.Key)
                {
                    _selectedItem = value;
                    OnNotifyPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Edition, Name);
        }
    }
}
