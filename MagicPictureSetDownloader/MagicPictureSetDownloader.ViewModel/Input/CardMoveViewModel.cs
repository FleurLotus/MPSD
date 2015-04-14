
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using MagicPictureSetDownloader.Interface;

    public class CardMoveViewModel : CardUpdateViewModelCommun
    {
        private ICardCollection _destinationCardCollectionSelected;
        private readonly ICardCollection[] _collections;

        public CardMoveViewModel(string collectionName, ICard card): 
            base(collectionName, card)
        {
            _collections = MagicDatabase.GetAllCollections().Where(c => c != SourceCardCollection)
                .ToArray();

            if (_collections.Length > 0)
                DestinationCardCollectionSelected = _collections[0];

            Display.Title = "Move card";
        }
        public ICardCollection[] Collections
        {
            get { return _collections; }
        }
        public ICardCollection DestinationCardCollectionSelected
        {
            get { return _destinationCardCollectionSelected; }
            set
            {
                if (value != _destinationCardCollectionSelected)
                {
                    _destinationCardCollectionSelected = value;
                    OnNotifyPropertyChanged(() => DestinationCardCollectionSelected);
                }
            }
        }
        protected override bool OkCommandCanExecute(object o)
        {
            if (Count <= 0 || Count > MaxCount || SourceEditionSelected == null)
                return false;

            return DestinationCardCollectionSelected != null && DestinationCardCollectionSelected != SourceCardCollection;
        }
    }
}
