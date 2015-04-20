
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using MagicPictureSetDownloader.Interface;

    public class CardMoveViewModel : UpdateViewModelCommun
    {
        private ICardCollection _cardCollectionSelected;
        private readonly ICardCollection[] _collections;

        public CardMoveViewModel(string collectionName, ICard card) : 
            base(collectionName)
        {
            Source = new CardSourceViewModel(MagicDatabase, SourceCollection, card);

            _collections = MagicDatabase.GetAllCollections().Where(c => c != SourceCollection)
                .ToArray();

            if (_collections.Length > 0)
                CardCollectionSelected = _collections[0];

            Display.Title = "Move card";
        }
        public CardSourceViewModel Source { get; private set; }
        public ICardCollection[] Collections
        {
            get { return _collections; }
        }
        public ICardCollection CardCollectionSelected
        {
            get { return _cardCollectionSelected; }
            set
            {
                if (value != _cardCollectionSelected)
                {
                    _cardCollectionSelected = value;
                    OnNotifyPropertyChanged(() => CardCollectionSelected);
                }
            }
        }
        protected override bool OkCommandCanExecute(object o)
        {
            if (Source.Count <= 0 || Source.Count > Source.MaxCount || Source.EditionSelected == null)
                return false;

            return CardCollectionSelected != null && CardCollectionSelected != SourceCollection;
        }
    }
}
