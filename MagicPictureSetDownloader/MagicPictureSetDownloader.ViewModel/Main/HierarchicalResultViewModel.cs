namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;

    using Common.ViewModel;

    public class HierarchicalResultViewModel : NotifyPropertyChangedBase
    {
        public HierarchicalResultViewModel(IComparable value)
        {
            Value = value;
            Children = new List<HierarchicalResultViewModel>();
        }

        public IComparable Value { get; }
        public string DisplayValue { get { return Value.ToString(); } }
        public IList<HierarchicalResultViewModel> Children { get; }
    }
}
