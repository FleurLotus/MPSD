namespace Common.WPF
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Collections.Generic;

    public static class VisualExtension
    {
        //From: https://gong-wpf-dragdrop.googlecode.com/svn-history/r29/branches/jon/GongSolutions.Wpf.DragDrop/Utilities/VisualTreeExtensions.cs
        public static T GetVisualAncestor<T>(this DependencyObject d) where T : class
        {
            DependencyObject item = VisualTreeHelper.GetParent(d);

            while (item != null)
            {
                T itemAsT = item as T;
                if (itemAsT != null)
                    return itemAsT;
                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }
        public static DependencyObject GetVisualAncestor(this DependencyObject d, Type type)
        {
            DependencyObject item = VisualTreeHelper.GetParent(d);

            while (item != null)
            {
                if (item.GetType() == type)
                    return item;
                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        public static T GetVisualDescendant<T>(this DependencyObject d) where T : DependencyObject
        {
            return d.GetVisualDescendants<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetVisualDescendants<T>(this DependencyObject d) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(d);

            for (int n = 0; n < childCount; n++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(d, n);

                if (child is T)
                {
                    yield return (T)child;
                }

                foreach (T match in GetVisualDescendants<T>(child))
                {
                    yield return match;
                }
            }
        }
    }
}
