namespace Common.WPF.Behavior
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    //From http://stackoverflow.com/questions/845269/force-resize-of-gridview-columns-inside-listview
    public class GridViewColumnResizeBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var listView = AssociatedObject;
            if (listView == null)
                return;

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
            var listView = AssociatedObject;
            if (listView == null)
                return;

            var gridView = listView.View as GridView;
            if (gridView == null)
                return;

            // If the column is automatically sized, change the column width to re-apply automatic width
            foreach (var column in gridView.Columns.Where(column => Double.IsNaN(column.Width)))
            {
                column.Width = column.ActualWidth;
                column.Width = Double.NaN;
            }
        }

        protected override void OnDetaching()
        {
            var listView = AssociatedObject;
            if (listView != null)
                RemoveHandler(listView.Items);

            base.OnDetaching();
        }
    }
}
