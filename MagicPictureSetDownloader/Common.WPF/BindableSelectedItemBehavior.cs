

namespace Common.WPF
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    //From http://stackoverflow.com/questions/11065995/binding-selecteditem-in-a-hierarchicaldatatemplate-applied-wpf-treeview/18700099#18700099
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior),
            new UIPropertyMetadata(null, OnSelectedItemChanged));

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
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }

        private static Action<int> GetBringIndexIntoView(Panel itemsHostPanel)
        {
            VirtualizingStackPanel virtualizingPanel = itemsHostPanel as VirtualizingStackPanel;
            if (virtualizingPanel == null)
                return null;

            MethodInfo method = virtualizingPanel.GetType().GetMethod("BringIndexIntoView", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, new[] { typeof(int) }, null);
            if (method == null)
                return null;

            return i => method.Invoke(virtualizingPanel, new object[] { i });
        }

        private static TreeViewItem GetTreeViewItem(ItemsControl container, object item)
        {
            if (container != null)
            {
                if (container.DataContext == item)
                {
                    return container as TreeViewItem;
                }

                // Expand the current container
                if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
                {
                    container.SetValue(TreeViewItem.IsExpandedProperty, true);
                }

                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the 
                // virtualizing case even if the item is marked 
                // expanded we still need to do this step in order to 
                // regenerate the visuals because they may have been virtualized away.
                container.ApplyTemplate();
                ItemsPresenter itemsPresenter = (ItemsPresenter)container.Template.FindName("ItemsHost", container);
                if (itemsPresenter != null)
                {
                    itemsPresenter.ApplyTemplate();
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter, 
                    // so walk the descendents and find the child.
                    itemsPresenter = container.GetVisualDescendant<ItemsPresenter>();
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();
                        itemsPresenter = container.GetVisualDescendant<ItemsPresenter>();
                    }
                }

                Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

                // Ensure that the generator for this panel has been created.
#pragma warning disable 168
                UIElementCollection children = itemsHostPanel.Children;
#pragma warning restore 168

                Action<int> bringIndexIntoView = GetBringIndexIntoView(itemsHostPanel);
                for (int i = 0, count = container.Items.Count; i < count; i++)
                {
                    TreeViewItem subContainer;
                    if (bringIndexIntoView != null)
                    {
                        // Bring the item into view so 
                        // that the container will be generated.
                        bringIndexIntoView(i);
                        subContainer = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);

                        // Bring the item into view to maintain the 
                        // same behavior as with a virtualizing panel.
                        subContainer.BringIntoView();
                    }

                    if (subContainer == null)
                    {
                        continue;
                    }

                    // Search the next level for the object.
                    TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);
                    if (resultContainer != null)
                    {
                        return resultContainer;
                    }

                    // The object is not under this TreeViewItem
                    // so collapse it.
                    subContainer.IsExpanded = false;
                }
            }

            return null;
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = e.NewValue as TreeViewItem;
            if (item != null)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
                return;
            }

            BindableSelectedItemBehavior behavior = (BindableSelectedItemBehavior)sender;
            TreeView treeView = behavior.AssociatedObject;
            if (treeView == null)
            {
                // at designtime the AssociatedObject sometimes seems to be null
                return;
            }

            item = GetTreeViewItem(treeView, e.NewValue);
            if (item != null)
            {
                item.IsSelected = true;
            }
        }
    }
}