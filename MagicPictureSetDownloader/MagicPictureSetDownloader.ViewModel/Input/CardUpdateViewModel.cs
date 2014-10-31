
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;


    public class CardUpdateViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler Closing;

        private ICardCollection _destinationCardCollectionSelected;
        private readonly ICardCollection[] _collections;
        private IEdition _sourceEditionSelected;
        private IEdition _destinationEditionSelected;
        private bool _sourceIsFoil;
        private bool _destinationIsFoil;
        private int _count;
        private int _maxCount;
        private readonly IEdition[] _editions;
        private readonly ICardInCollectionCount[] _cardInCollectionCounts;
        private readonly IMagicDatabaseReadOnly _magicDatabase;


        public CardUpdateViewModel(string collectionName, ICard card, bool forMoving)
        {
            Card = card;

            ForMoving = forMoving;
            UpdateCommand = new RelayCommand(UpdateCommandExecute, UpdateCommandCanExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);

            _magicDatabase = MagicDatabaseManager.ReadOnly;

            SourceCardCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == collectionName);
            _collections = _magicDatabase.GetAllCollections().Where(c => c != SourceCardCollection).ToArray();
            _cardInCollectionCounts = _magicDatabase.GetCardCollectionStatistics(Card).Where(cicc => cicc.IdCollection == SourceCardCollection.Id).ToArray();
            _editions = _cardInCollectionCounts.Select(cicc => _magicDatabase.GetEdition(cicc.IdGatherer)).ToArray();

            if (_editions.Length > 0)
            {
                SourceEditionSelected = _editions[0];
                if (!ForMoving)
                    DestinationEditionSelected = SourceEditionSelected;
            }

            if (ForMoving)
            {
                if (_collections.Length > 0)
                    DestinationCardCollectionSelected = _collections[0];
            }
        }
        public ICommand UpdateCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public bool? Result { get; private set; }
        public bool ForMoving { get; private set; }
        public ICard Card { get; private set; }
        public IEdition[] Editions
        {
            get { return _editions; }
        }
        public ICardCollection[] Collections
        {
            get { return _collections; }
        }
        public ICardCollection SourceCardCollection { get; private set; }

        public bool SourceIsFoil
        {
            get { return _sourceIsFoil; }
            set
            {
                if (value != _sourceIsFoil)
                {
                    _sourceIsFoil = value;
                    OnNotifyPropertyChanged(() => SourceIsFoil);
                    UpdateMaxCount();
                }
            }
        }
        public IEdition SourceEditionSelected
        {
            get { return _sourceEditionSelected; }
            set
            {
                if (value != _sourceEditionSelected)
                {
                    _sourceEditionSelected = value;
                    OnNotifyPropertyChanged(() => SourceEditionSelected);
                    UpdateMaxCount();
                }
            }
        }
        public bool DestinationIsFoil
        {
            get { return _destinationIsFoil; }
            set
            {
                if (value != _destinationIsFoil)
                {
                    _destinationIsFoil = value;
                    OnNotifyPropertyChanged(() => DestinationIsFoil);
                }
            }
        }
        public IEdition DestinationEditionSelected
        {
            get { return _destinationEditionSelected; }
            set
            {
                if (value != _destinationEditionSelected)
                {
                    _destinationEditionSelected = value;
                    OnNotifyPropertyChanged(() => DestinationEditionSelected);
                    if (_destinationEditionSelected != null && !_destinationEditionSelected.HasFoil)
                        DestinationIsFoil = false;
                }
            }
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
        public int Count
        {
            get { return _count; }
            set
            {
                if (value != _count)
                {
                    _count = value;
                    OnNotifyPropertyChanged(() => Count);
                }
            }
        }
        public int MaxCount
        {
            get { return _maxCount; }
            set
            {
                if (value != _maxCount)
                {
                    _maxCount = value;
                    OnNotifyPropertyChanged(() => MaxCount);

                    if (value < Count)
                        Count = value;
                }
            }
        }

        private void UpdateMaxCount()
        {

            int idGatherer = _magicDatabase.GetIdGatherer(Card, SourceEditionSelected);
            ICardInCollectionCount cardInCollectionCount = _cardInCollectionCounts.FirstOrDefault(cicc => cicc.IdGatherer == idGatherer);
            if (cardInCollectionCount == null)
            {
                MaxCount = 0;
                return;
            }
            MaxCount = SourceIsFoil? cardInCollectionCount.FoilNumber: cardInCollectionCount.Number;
        }

        private void UpdateCommandExecute(object o)
        {
            Result = true;
            OnClosing();
        }
        private void CloseCommandExecute(object o)
        {
            Result = false;
            OnClosing();
        }
        private bool UpdateCommandCanExecute(object o)
        {
            if (Count <= 0 || Count > MaxCount || SourceEditionSelected == null)
                return false;

            if (ForMoving)
                return DestinationCardCollectionSelected != null && DestinationCardCollectionSelected != SourceCardCollection;

            return DestinationEditionSelected != null && (DestinationEditionSelected != SourceEditionSelected || SourceIsFoil != DestinationIsFoil) && (DestinationEditionSelected.HasFoil || !DestinationIsFoil);
        }
        private void OnClosing()
        {
            var e = Closing;
            if (e != null)
                e(this, EventArgs.Empty);
        }
    }
}
