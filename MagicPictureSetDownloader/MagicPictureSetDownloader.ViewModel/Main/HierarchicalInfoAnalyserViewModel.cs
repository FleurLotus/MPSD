namespace MagicPictureSetDownloader.ViewModel.Main
{
    using Common.ViewModel;

    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;

    public class HierarchicalInfoAnalyserViewModel : NotifyPropertyChangedBase
    {
        private bool _isActive;
        private bool _isAscendingOrder;

        public HierarchicalInfoAnalyserViewModel(string name)
        {
            Name = name;
            IsAscendingOrder = true;
            IsActive = true;
            Analyser = HierarchicalInfoAnalyserFactory.Instance.Create(name);
        }

        public string Name { get; private set; }
        public IHierarchicalInfoAnalyser Analyser { get; private set; }

        public bool IsAscendingOrder
        {
            get { return _isAscendingOrder; }
            set
            {
                if (value != _isAscendingOrder)
                {
                    _isAscendingOrder = value;
                    OnNotifyPropertyChanged(() => IsAscendingOrder);
                }
            }
        }
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