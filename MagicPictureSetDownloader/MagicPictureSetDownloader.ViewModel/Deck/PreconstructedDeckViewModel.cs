namespace MagicPictureSetDownloader.ViewModel.Deck
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Common.ViewModel;
    using MagicPictureSetDownloader.ViewModel.Main;

    public class PreconstructedDeckViewModel: NotifyPropertyChangedBase
    {
        private KeyValuePair<CardViewModel, int> _selectedItem;

        public PreconstructedDeckViewModel(string edition, string name, IEnumerable<KeyValuePair<CardViewModel, int>> cards)
        {
            Name = name;
            Edition = edition;
            IDictionary<CardViewModel, int> temp = new Dictionary<CardViewModel, int>();
            foreach (KeyValuePair<CardViewModel, int> kv in cards)
            {
                temp.Add(kv);
            }
            Composition = new ReadOnlyDictionary<CardViewModel, int>(temp);
        }

        public string Name { get; }
        public string Edition { get; }
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
