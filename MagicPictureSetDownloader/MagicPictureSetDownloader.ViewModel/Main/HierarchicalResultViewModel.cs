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
            Children = new Collection<HierarchicalResultViewModel>();
        }

        public IComparable Value { get; private set; }
        public string DisplayValue { get { return Value.ToString(); } }
        public Collection<HierarchicalResultViewModel> Children { get; private set; }
    }
}
