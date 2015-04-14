
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;


    public class CardUpdateViewModelCommun : DialogViewModelBase
    {
        private IEdition _sourceEditionSelected;
        private ILanguage _sourceLanguageSelected;
        private ILanguage[] _sourceLanguages;

        private bool _sourceIsFoil;
        private int _count;
        private int _maxCount;
        private readonly IEdition[] _sourceEditions;
        private readonly ICardInCollectionCount[] _cardInCollectionCounts;
        protected readonly IMagicDatabaseReadOnly MagicDatabase;

        protected CardUpdateViewModelCommun(string collectionName, ICard card)
        {
            Card = card;

            MagicDatabase = MagicDatabaseManager.ReadOnly;

            SourceCardCollection = MagicDatabase.GetAllCollections().First(cc => cc.Name == collectionName);

            _cardInCollectionCounts = MagicDatabase.GetCollectionStatisticsForCard(SourceCardCollection, Card)
                .ToArray();

            _sourceEditions = _cardInCollectionCounts.Select(cicc => MagicDatabase.GetEdition(cicc.IdGatherer))
                .Distinct()
                .Ordered()
                .ToArray();

            if (_sourceEditions.Length > 0)
            {
                SourceEditionSelected = _sourceEditions[0];
            }
            Display.OkCommandLabel = "Update";
            Display.CancelCommandLabel = "Close";
        }
        public ICard Card { get; private set; }
        public IEdition[] SourceEditions
        {
            get { return _sourceEditions; }
        }
        public ICardCollection SourceCardCollection { get; private set; }

        public ILanguage[] SourceLanguages
        {
            get { return _sourceLanguages; }
            private set
            {
                if (value != _sourceLanguages)
                {
                    _sourceLanguages = value;
                    OnNotifyPropertyChanged(() => SourceLanguages);
                    if (_sourceLanguages != null && _sourceLanguages.Length > 0)
                        SourceLanguageSelected = _sourceLanguages[0];
                }
            }
        }

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
                    ChangeSourceLanguage();
                    UpdateMaxCount();
                }
            }
        }
        public ILanguage SourceLanguageSelected
        {
            get { return _sourceLanguageSelected; }
            set
            {
                if (value != _sourceLanguageSelected)
                {
                    _sourceLanguageSelected = value;
                    OnNotifyPropertyChanged(() => SourceLanguageSelected);
                    UpdateMaxCount();
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
            int idGatherer = MagicDatabase.GetIdGatherer(Card, SourceEditionSelected);
            ICardInCollectionCount cardInCollectionCount = _cardInCollectionCounts.FirstOrDefault(cicc => cicc.IdGatherer == idGatherer && cicc.IdLanguage == SourceLanguageSelected.Id);
          
            if (cardInCollectionCount == null)
            {
                MaxCount = 0;
                return;
            }

            MaxCount = SourceIsFoil? cardInCollectionCount.FoilNumber: cardInCollectionCount.Number;
        }

        private void ChangeSourceLanguage()
        {
            int idGatherer = MagicDatabase.GetIdGatherer(Card, SourceEditionSelected);
            SourceLanguages = MagicDatabase.GetLanguages(idGatherer).ToArray();
        }
    }
}
