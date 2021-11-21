namespace Common.WPF
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;

    //base on http://stackoverflow.com/questions/9825622/silverlight-tabitem-visibility-not-changing/13276564#13276564
    public static class TabItemExtensions
    {
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached("Visibility", typeof(Visibility), typeof(TabItemExtensions), new PropertyMetadata(Visibility.Visible, VisibilityChanged));
        
        /// <summary>
        /// Use this property in a TabItem instead of the original "Visibility" to 
        /// correct the behavior of a TabControl when a TabItem's Visibility changes.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Visibility GetVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(VisibilityProperty);
        }
        public static void SetVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(VisibilityProperty, value);
        }

        public static void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not TabItem tabItem)
            {
                return;
            }

            var visibility = (Visibility)args.NewValue;
            if (tabItem.Visibility == visibility)
            {
                return;
            }

            tabItem.Visibility = visibility;
            if (visibility == Visibility.Visible)
            {
                return;
            }

            // Finds the tab's parent tabcontrol and corrects the selected item, 
            // if necessary.
            var tabControl = tabItem.GetSelfAndAncestors().OfType<TabControl>().FirstOrDefault();
            if (tabControl == null)
            {
                return;
            }

            TabControlExtensions.CorrectSelection(tabControl);
        }
    }
}
