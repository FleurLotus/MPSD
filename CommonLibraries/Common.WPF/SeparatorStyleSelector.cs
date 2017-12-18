namespace Common.WPF
{
    using System.Windows;
    using System.Windows.Controls;

    using Common.ViewModel.Menu;

    //Use to allow display of separator with menu created from binding
    public class SeparatorStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            MenuViewModel menu = item as MenuViewModel;

            if (menu != null && menu.IsSeparator)
            {
                ResourceDictionary resourceDictionary = Application.Current.Resources;
                if (resourceDictionary.Contains("separatorStyle"))
                {
                    return (Style)resourceDictionary["separatorStyle"];
                }
            }
            return null;
        }
    }
}
