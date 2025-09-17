namespace Common.WPF.Attach.UI.UnitTests
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Common.ViewModel;
    using Common.ViewModel.Command;

    public record class GridViewSortItem(string Name, int Value);

    public class GridViewSortWindowViewModel : NotifyPropertyChangedBase
    {
        public GridViewSortWindowViewModel()
        {
            Elements = new ObservableCollection<GridViewSortItem>
            {
                new GridViewSortItem("Alan", 100),
                new GridViewSortItem("Bob", 150),
                new GridViewSortItem("Charlie", 90),
                new GridViewSortItem("David", 130),
            };

            Elements2 = new ObservableCollection<GridViewSortItem>
            {
                new GridViewSortItem("Alan", 100),
                new GridViewSortItem("Bob", 150),
                new GridViewSortItem("Charlie", 90),
                new GridViewSortItem("David", 130),
            };

            AddCommand = new RelayCommand(o => Elements2.Add(new GridViewSortItem("Edward" + o, 200)));
        }
        public ObservableCollection<GridViewSortItem> Elements { get; }
        public ObservableCollection<GridViewSortItem> Elements2 { get; }
        public ICommand AddCommand { get; }
    }
}