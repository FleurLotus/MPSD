namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.ObjectModel;

    using Common.ViewModel;

    public class HierarchicalResultViewModel : NotifyPropertyChangedBase
    {
        public HierarchicalResultViewModel(IComparable value)
        {
            Value = value;
            Children = new ObservableCollection<HierarchicalResultViewModel>();
        }

        public IComparable Value { get; private set; }
        public string DisplayValue { get { return Value.ToString(); } }
        public ObservableCollection<HierarchicalResultViewModel> Children { get; private set; }
    }
}
