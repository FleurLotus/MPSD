namespace Common.WPF.UI.UnitTests
{
    using Common.ViewModel;
    using Common.ViewModel.Menu;

    public class SeparatorStyleSelectorWindowViewModel : NotifyPropertyChangedBase
    {
        public SeparatorStyleSelectorWindowViewModel()
        {
            MenuRoot = new MenuViewModel();

            MenuViewModel fileMenu = new MenuViewModel("_File");
            fileMenu.AddChild(new MenuViewModel("Item1"));
            fileMenu.AddChild(new MenuViewModel("Item2"));
            fileMenu.AddChild(MenuViewModel.Separator());
            fileMenu.AddChild(new MenuViewModel("Item3"));

            MenuRoot.AddChild(fileMenu);
        }
        public MenuViewModel MenuRoot { get; }
    }
}