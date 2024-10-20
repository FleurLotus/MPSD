﻿namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public class CardUpdateViewModel : UpdateViewModelCommun
    {
        private IEdition _editionSelected;
        private ILanguage _languageSelected;
        private ILanguage[] _languages;

        private bool _isFoil;
        private bool _isAltArt;
        private readonly IEdition[] _editions;

        public CardUpdateViewModel(string collectionName, ICard card)
            : base(collectionName)
        {
            _editions = MagicDatabase.GetAllEditionIncludingCardOrdered(card)
                .ToArray();

            Source = new CardSourceViewModel(MagicDatabase, SourceCollection, card);

            EditionSelected = Source.EditionSelected;

            Display.Title = "Update infos";
        }
        public CardSourceViewModel Source { get; }
        public IEdition[] Editions
        {
            get { return _editions; }
        }
        public ILanguage[] Languages
        {
            get { return _languages; }
            private set
            {
                if (value != _languages)
                {
                    _languages = value;
                    OnNotifyPropertyChanged(nameof(Languages));
                    if (_languages != null && _languages.Length > 0)
                    {
                        LanguageSelected = _languages[0];
                    }
                }
            }
        }
        public bool IsFoil
        {
            get { return _isFoil; }
            set
            {
                if (value != _isFoil)
                {
                    _isFoil = value;
                    OnNotifyPropertyChanged(nameof(IsFoil));
                }
            }
        }
        public bool IsAltArt
        {
            get { return _isAltArt; }
            set
            {
                if (value != _isAltArt)
                {
                    _isAltArt = value;
                    OnNotifyPropertyChanged(nameof(IsAltArt));
                }
            }
        }
        public IEdition EditionSelected
        {
            get { return _editionSelected; }
            set
            {
                if (value != _editionSelected)
                {
                    _editionSelected = value;
                    OnNotifyPropertyChanged(nameof(EditionSelected));
                    ChangeDestinationLanguage();
                    if (_editionSelected != null && !_editionSelected.HasFoil)
                    {
                        IsFoil = false;
                    }
                }
            }
        }
        public ILanguage LanguageSelected
        {
            get { return _languageSelected; }
            set
            {
                if (value != _languageSelected)
                {
                    _languageSelected = value;
                    OnNotifyPropertyChanged(nameof(LanguageSelected));
                }
            }
        }
        
        private void ChangeDestinationLanguage()
        {
            string idScryFall = MagicDatabase.GetIdScryFall(Source.Card, EditionSelected);
            Languages = MagicDatabase.GetLanguages(idScryFall).ToArray();
        }
        protected override bool OkCommandCanExecute(object o)
        {
            if (Source.Count <= 0 || Source.Count > Source.MaxCount || Source.EditionSelected == null)
            {
                return false;
            }

            return EditionSelected != null && LanguageSelected!= null &&
                   (LanguageSelected != Source.LanguageSelected || EditionSelected != Source.EditionSelected || IsFoil != Source.IsFoil || IsAltArt != Source.IsAltArt) && 
                   (EditionSelected.HasFoil || !IsFoil);
        }
    }
}
