namespace Common.WPF
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    //from http://stackoverflow.com/questions/1000040/data-binding-to-selecteditem-in-a-wpf-treeview
    //more complex version exists  http://stackoverflow.com/questions/11065995/binding-selecteditem-in-a-hierarchicaldatatemplate-applied-wpf-treeview/18700099#18700099
    //for TwoWay with HierarchicalDataTemplate
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof (object), typeof (BindableSelectedItemBehavior));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }
    }
}
