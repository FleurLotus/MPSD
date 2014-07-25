namespace MagicPictureSetDownloader.ViewModel
{
    using System.Collections.ObjectModel;
    using Common.ViewModel;

    public class HierarchicalResultViewModel : NotifyPropertyChangedBase
    {
        public HierarchicalResultViewModel(string name)
        {
            Name = name;
            Children = new ObservableCollection<HierarchicalResultViewModel>();
        }

        public string Name { get; private set; }
        public ObservableCollection<HierarchicalResultViewModel> Children { get; private set; }
    }
}
