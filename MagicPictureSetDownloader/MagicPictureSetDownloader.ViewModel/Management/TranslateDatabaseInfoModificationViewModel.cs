
namespace MagicPictureSetDownloader.ViewModel.Management
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public class TranslateDatabaseInfoModificationViewModel : DatabaseInfoModificationViewModelBase<ICard>
    {
        private readonly ILanguage[] _allLanguages;
        private readonly ILanguage[] _notUpdatableLanguages;
        private TranslationViewModel _defaultTranslation;

        public TranslateDatabaseInfoModificationViewModel()
        {
            AllTranslations = new ObservableCollection<TranslationViewModel>();
            All.AddRange(MagicDatabase.GetAllInfos().GetAllCardsOrderByTranslation(null).Select(kv=> kv.Value));
            _notUpdatableLanguages = new[] { MagicDatabase.GetDefaultLanguage(), MagicDatabase.GetEnglishLanguage() };
            _allLanguages = MagicDatabase.GetAllLanguages().Where(l=> !_notUpdatableLanguages.Contains(l)).ToArray();
            Title = "Manage Translate";
        }
        public ICollection<TranslationViewModel> AllTranslations { get; private set; }
        public TranslationViewModel DefaultTranslation
        {
            get { return _defaultTranslation; }
            private set
            {
                if (_defaultTranslation != value)
                {
                    _defaultTranslation = value;
                    OnNotifyPropertyChanged(() => DefaultTranslation);
                }
            }
        }
        public override bool AllowNew
        {
            get { return false; }
        }

        protected override void DisplayCurrent()
        {
            if (Selected == null)
            {
                Name = null;
            }
            else
            {
                Name = Selected.ToString();
            }
            BuildAllTranslations();
        }
        protected override bool CouldBeUpdated()
        {
            return base.CouldBeUpdated() && AllTranslations.Count > 0;
        }
        protected override bool ValidateCurrent()
        {
            return base.ValidateCurrent() && AllTranslations.Any(t => t.Modified) && AllTranslations.All(t => !string.IsNullOrWhiteSpace(t.Translation));
        }
        protected override bool ApplyEditionToDatabase()
        {
            foreach (TranslationViewModel translation in AllTranslations.Where(t=>t.Modified))
            {
                MagicDatabase.UpdateTranslate(Selected, translation.Language, translation.Translation);
            }
            return true;
        }
        private void BuildAllTranslations()
        {
            AllTranslations.Clear();
            DefaultTranslation = null;

            if (Selected == null)
                return;

            //English
            DefaultTranslation = new TranslationViewModel(_notUpdatableLanguages[1], Selected.ToString());

            foreach (ILanguage language in _allLanguages)
            {
                string translation = Selected.ToString(language.Id);
                if (!string.IsNullOrEmpty(translation))
                {
                    AllTranslations.Add(new TranslationViewModel(language, translation));
                }
            }
        }
    }
}
