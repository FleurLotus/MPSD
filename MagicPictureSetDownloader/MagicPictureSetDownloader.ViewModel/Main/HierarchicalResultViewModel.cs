namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
//    using System.Collections.ObjectModel;

    using Common.ViewModel;

    public class HierarchicalResultViewModel : NotifyPropertyChangedBase
    {
        public HierarchicalResultViewModel(IComparable value)
        {
            Value = value;
            Children = new List<HierarchicalResultViewModel>();
        }

        public IComparable Value { get; private set; }
        public string DisplayValue { get { return Value.ToString(); } }
        public IList<HierarchicalResultViewModel> Children { get; private set; }
    }
}
