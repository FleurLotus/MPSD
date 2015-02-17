
namespace MagicPictureSetDownloader.ViewModel.Management
{
    using MagicPictureSetDownloader.Interface;

    public class LanguageDatabaseInfoModificationViewModel : DatabaseInfoModificationViewModelBase<ILanguage>
    {
        private string _alternativeName;

        public LanguageDatabaseInfoModificationViewModel()
        {
            All.AddRange(MagicDatabase.GetAllLanguages());
            Title = "Manage Language";
        }

        public string AlternativeName
        {
            get { return _alternativeName; }
            set
            {
                if (value != _alternativeName)
                {
                    _alternativeName = value;
                    OnNotifyPropertyChanged(() => AlternativeName);
                }
            }
        }

        protected override bool CheckCurrent()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
        protected override void DisplayCurrent()
        {
            if (Selected == null)
            {
                AlternativeName = null;
                Name = null;
            }
            else
            {
                AlternativeName = Selected.AlternativeName;
                Name = Selected.Name;
            }
        }
        protected override bool ValidateEdition()
        {
            if (Selected == null)
            {
                MagicDatabase.InsertNewLanguage(Name, AlternativeName);
            }
            else
            {
                MagicDatabase.UpdateLanguage(Selected, Name, AlternativeName);
            }
            All.Clear();
            All.AddRange(MagicDatabase.GetAllLanguages());
            return true;
        }
    }
}
