namespace Common.WPF.Behavior.UI.UnitTests
{
    using System.Collections.ObjectModel;

    using Common.ViewModel;

    public class BindableSelectedItemBehaviorWindowViewModel : NotifyPropertyChangedBase
    {
        private TreeViewModel _selected;

        public BindableSelectedItemBehaviorWindowViewModel()
        {
            Root = new ObservableCollection<TreeViewModel>();
            TreeViewModel rootItem = new TreeViewModel("Root");
            Root.Add(rootItem);
            TreeViewModel c = new TreeViewModel("Child 1");
            c.Children.Add(new TreeViewModel("Child 1.1"));
            c.Children.Add(new TreeViewModel("Child 1.2"));
            rootItem.Children.Add(c);
            c = new TreeViewModel("Child 2");
            c.Children.Add(new TreeViewModel("Child 2.1"));
            c.Children.Add(new TreeViewModel("Child 2.2"));
            TreeViewModel c2 = new TreeViewModel("Child 2.3");
            c2.Children.Add(new TreeViewModel("Child 2.3.1"));
            c2.Children.Add(new TreeViewModel("Child 2.3.2"));
            c.Children.Add(c2);
            rootItem.Children.Add(c);
            rootItem.Children.Add(new TreeViewModel("Child 3"));
        }

        public ObservableCollection<TreeViewModel> Root { get; }
        public TreeViewModel Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    OnNotifyPropertyChanged();
                }
            }
        }
    }

    public class TreeViewModel(string value) : NotifyPropertyChangedBase
    {
        public string DisplayValue { get; } = value;
        public ObservableCollection<TreeViewModel> Children { get; } = new ObservableCollection<TreeViewModel>();
    }
}