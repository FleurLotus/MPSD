namespace MagicPictureSetDownloader.ViewModel
{
    using Common.ViewModel;
    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;

    class HierarchicalInfoAnalyserViewModel: NotifyPropertyChangedBase
    {
        private bool _isActive;

        public HierarchicalInfoAnalyserViewModel(string name)
        {
            Name = name;
            Analyser = HierarchicalInfoAnalyserFactory.Instance.Create(name);
        }

        public string Name { get; private set; }
        public IHierarchicalInfoAnalyser Analyser { get; private set; }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (value != _isActive)
                {
                    _isActive = value;
                    OnNotifyPropertyChanged(() => IsActive);
                }
            }
        }

    }
}
