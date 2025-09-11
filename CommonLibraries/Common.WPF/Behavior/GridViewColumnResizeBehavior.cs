namespace Common.WPF.Behavior
{
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Controls;

    using Microsoft.Xaml.Behaviors;

    //From http://stackoverflow.com/questions/845269/force-resize-of-gridview-columns-inside-listview
    public class GridViewColumnResizeBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            ListView listView = AssociatedObject;
            if (listView == null)
            {
                return;
            }
            AddHandler(listView.Items);
        }

        private void AddHandler(INotifyCollectionChanged sourceCollection)
        {
            sourceCollection.CollectionChanged += OnListViewItemsCollectionChanged;
        }

        private void RemoveHandler(INotifyCollectionChanged sourceCollection)
        {
            sourceCollection.CollectionChanged -= OnListViewItemsCollectionChanged;
        }

        private void OnListViewItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            ListView listView = AssociatedObject;
            if (listView == null)
            {
                return;
            }

            if (listView.View is not GridView gridView)
            {
                return;
            }

            // If the column is automatically sized, change the column width to re-apply automatic width
            foreach (GridViewColumn column in gridView.Columns.Where(column => double.IsNaN(column.Width)))
            {
                column.Width = column.ActualWidth;
                column.Width = double.NaN;
            }
        }

        protected override void OnDetaching()
        {
            ListView listView = AssociatedObject;
            if (listView != null)
            {
                RemoveHandler(listView.Items);
            }

            base.OnDetaching();
        }
    }
}