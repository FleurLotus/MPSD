namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using MagicPictureSetDownloader.Interface;

    public class CardMoveOrCopyViewModel : UpdateViewModelCommun
    {
        private ICardCollection _cardCollectionSelected;
        private bool _copy;

        public CardMoveOrCopyViewModel(string collectionName, ICard card, bool copy) :
            base(collectionName)
        {
            Source = new CardSourceViewModel(MagicDatabase, SourceCollection, card);

            Copy = copy;
            Collections = MagicDatabase.GetAllCollections().ToArray();

            if (Collections.Length > 0)
            {
                if (SourceCollection != Collections[0] || Collections.Length == 1)
                {
                    CardCollectionSelected = Collections[0];
                }
                else
                {
                    CardCollectionSelected = Collections[1];
                }
            }
        }
        public CardSourceViewModel Source { get; }
        public ICardCollection[] Collections { get; }
        public ICardCollection CardCollectionSelected
        {
            get { return _cardCollectionSelected; }
            set
            {
                if (value != _cardCollectionSelected)
                {
                    _cardCollectionSelected = value;
                    OnNotifyPropertyChanged(nameof(CardCollectionSelected));
                }
            }
        }
        public bool Copy
        {
            get { return _copy; }
            set
            {
                if (value != _copy)
                {
                    _copy = value;
                    OnNotifyPropertyChanged(nameof(Copy));
                }
                Display.Title = Copy ? "Copy card" : "Move card";
            }
        }

        protected override bool OkCommandCanExecute(object o)
        {
            if (Source.Count <= 0 || Source.Count > Source.MaxCount || Source.EditionSelected == null)
            {
                return false;
            }

            return CardCollectionSelected != null && (Copy || CardCollectionSelected != SourceCollection);
        }
    }
}