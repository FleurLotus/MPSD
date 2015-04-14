
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public class CardUpdateViewModel : CardUpdateViewModelCommun
    {
        private IEdition _destinationEditionSelected;
        private ILanguage _destinationLanguageSelected;
        private ILanguage[] _destinationLanguages;

        private bool _destinationIsFoil;
        private readonly IEdition[] _destinationEditions;
        
        public CardUpdateViewModel(string collectionName, ICard card): base (collectionName, card)
        {
            _destinationEditions = MagicDatabase.GetAllEditionIncludingCardOrdered(card)
                                                 .ToArray();

            DestinationEditionSelected = SourceEditionSelected;

            Display.Title = "Update infos";
        }
        public IEdition[] DestinationEditions
        {
            get { return _destinationEditions; }
        }
        public ILanguage[] DestinationLanguages
        {
            get { return _destinationLanguages; }
            private set
            {
                if (value != _destinationLanguages)
                {
                    _destinationLanguages = value;
                    OnNotifyPropertyChanged(() => DestinationLanguages);
                    if (_destinationLanguages != null && _destinationLanguages.Length > 0)
                        DestinationLanguageSelected = _destinationLanguages[0];
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
                    ChangeDestinationLanguage();
                    if (_destinationEditionSelected != null && !_destinationEditionSelected.HasFoil)
                        DestinationIsFoil = false;
                }
            }
        }
        public ILanguage DestinationLanguageSelected
        {
            get { return _destinationLanguageSelected; }
            set
            {
                if (value != _destinationLanguageSelected)
                {
                    _destinationLanguageSelected = value;
                    OnNotifyPropertyChanged(() => DestinationLanguageSelected);
                }
            }
        }
        private void ChangeDestinationLanguage()
        {
            int idGatherer = MagicDatabase.GetIdGatherer(Card, DestinationEditionSelected);
            DestinationLanguages = MagicDatabase.GetLanguages(idGatherer).ToArray();
        }
        protected override bool OkCommandCanExecute(object o)
        {
            if (Count <= 0 || Count > MaxCount || SourceEditionSelected == null)
                return false;

            return DestinationEditionSelected != null && DestinationLanguageSelected!= null && 
                   (DestinationLanguageSelected != SourceLanguageSelected || DestinationEditionSelected != SourceEditionSelected || SourceIsFoil != DestinationIsFoil) && 
                   (DestinationEditionSelected.HasFoil || !DestinationIsFoil);
        }
    }
}
