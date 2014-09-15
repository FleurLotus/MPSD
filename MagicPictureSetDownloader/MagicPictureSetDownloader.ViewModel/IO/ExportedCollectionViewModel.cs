
namespace MagicPictureSetDownloader.ViewModel.IO
{
    using Common.ViewModel;

    public class ExportedCollectionViewModel: NotifyPropertyChangedBase
    {
        private bool _isSelected;

        public ExportedCollectionViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnNotifyPropertyChanged(() => IsSelected);
                }
            }
        }
    }
}
