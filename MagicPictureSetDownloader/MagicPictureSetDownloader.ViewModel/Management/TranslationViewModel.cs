namespace MagicPictureSetDownloader.ViewModel.Management
{
    using Common.ViewModel;

    using MagicPictureSetDownloader.Interface;

    public class TranslationViewModel: NotifyPropertyChangedBase
    {
        private string _translation;
        private readonly string _originalTranslation;

        public TranslationViewModel(ILanguage language, string originalTranslation)
        {
            AddLinkedProperty(() => Translation, () => Modified);

            Language = language;
            _originalTranslation = originalTranslation;
            Translation = originalTranslation;
        }
        public ILanguage Language { get; private set; }
        public bool Modified
        {
            get { return Translation != _originalTranslation; }
        }
        public string Translation
        {
            get { return _translation; }
            set
            {
                if (_translation != value)
                {
                    _translation = value;
                    OnNotifyPropertyChanged(() => Translation);
                }
            }
        }
    }
}
