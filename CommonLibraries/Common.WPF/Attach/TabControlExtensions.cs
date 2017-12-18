namespace Common.WPF
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    // based on http://stackoverflow.com/questions/9825622/silverlight-tabitem-visibility-not-changing/13276564#13276564
    public static class TabControlExtensions
    {
        public static readonly DependencyProperty SelectOnlyVisibleTabsProperty = DependencyProperty.RegisterAttached("SelectOnlyVisibleTabs", typeof(bool), typeof(TabControlExtensions), new PropertyMetadata(false, SelectOnlyVisibleTabsChanged));
        
        /// <summary>
        /// Use this property on a TabControl to correct the behavior
        /// of selecting Collapsed TabItems.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetSelectOnlyVisibleTabs(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectOnlyVisibleTabsProperty);
        }
        public static void SetSelectOnlyVisibleTabs(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectOnlyVisibleTabsProperty, value);
        }

        public static void SelectOnlyVisibleTabsChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var tabControl = sender as TabControl;
            if (tabControl == null) return;

            if ((bool)args.NewValue)
            {
                tabControl.SelectionChanged += TabControlSelectionChanged;
                CorrectSelection(tabControl);
            }
            else
            {
                tabControl.SelectionChanged -= TabControlSelectionChanged;
            }
        }

        private static void TabControlSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var tabControl = sender as TabControl;
            if (tabControl == null) return;

            CorrectSelection(tabControl);
        }

        public static void CorrectSelection(TabControl tabControl)
        {
            var selected = tabControl.SelectedItem as UIElement;
            if (selected == null) return;

            // If the selected element is not suposed to be visible,
            // selects the next visible element
            if (selected.Visibility == Visibility.Collapsed)
            {
                tabControl.SelectedItem = tabControl.Items.OfType<UIElement>().FirstOrDefault(e => e.Visibility == Visibility.Visible);
            }
        }
    }
}