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

        public string Name { get; }
        public IHierarchicalInfoAnalyser Analyser { get; }

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

        public string SaveInfo(int position)
        {
            return string.Format("{0},{1},{2}", IsAscendingOrder, IsActive, position);
        }
        public int LoadInfo(string value)
        {
            string[] data = value.Split(',');

            IsAscendingOrder = bool.Parse(data[0]);
            IsActive = bool.Parse(data[1]);
            return int.Parse(data[2]);
        }
    }
}