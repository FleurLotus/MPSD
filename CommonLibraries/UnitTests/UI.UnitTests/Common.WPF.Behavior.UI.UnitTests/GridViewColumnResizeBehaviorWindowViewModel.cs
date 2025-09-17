namespace Common.WPF.Behavior.UI.UnitTests
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Common.ViewModel;
    using Common.ViewModel.Command;

    public record class GridViewColumnResizeBehaviorItem(string Name, int Value);

    public class GridViewColumnResizeBehaviorWindowViewModel : NotifyPropertyChangedBase
    {
        public GridViewColumnResizeBehaviorWindowViewModel()
        {
            Elements = new ObservableCollection<GridViewColumnResizeBehaviorItem>
            {
                new GridViewColumnResizeBehaviorItem("Alan", 100),
                new GridViewColumnResizeBehaviorItem("Bob", 150),
                new GridViewColumnResizeBehaviorItem("Charlie", 90),
                new GridViewColumnResizeBehaviorItem("David", 130),
                new GridViewColumnResizeBehaviorItem("Edward", 200),
            };

            AddCommand = new RelayCommand(o => Elements.Add(new GridViewColumnResizeBehaviorItem("Fabadshdhsdhsdhshdshdhsdhs", 250)));
        }
        public ObservableCollection<GridViewColumnResizeBehaviorItem> Elements { get; }

        public ICommand AddCommand { get; }
    }
}