namespace Common.WPF.Behavior
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class FocusBehavior
    {
        #region FocusFirst
        //From http://stackoverflow.com/questions/817610/wpf-and-initial-focus
        public static readonly DependencyProperty FocusFirstProperty = DependencyProperty.RegisterAttached("FocusFirst", typeof(bool), typeof(FocusBehavior), new PropertyMetadata(false, OnFocusFirstChanged));
        public static bool GetFocusFirst(Control control)
        {
            return (bool)control.GetValue(FocusFirstProperty);
        }
        public static void SetFocusFirst(Control control, bool value)
        {
            control.SetValue(FocusFirstProperty, value);
        }

        private static void OnFocusFirstChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not Control control || args.NewValue is not bool)
            {
                return;
            }

            if ((bool)args.NewValue)
            {
                control.Loaded += (sender, e) => control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
        #endregion

        #region IsFocused
        //From http://stackoverflow.com/questions/19657951/how-to-set-focus-to-a-wpf-control-using-mvvm
        public static DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusBehavior), new UIPropertyMetadata(false, OnIsFocusedChanged));

        public static bool GetIsFocused(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsFocusedProperty, value);
        }

        public static void OnIsFocusedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not Control control || args.NewValue is not bool)
            {
                return;
            }

            bool newValue = (bool)args.NewValue;
            bool oldValue = (bool)args.OldValue;
            if (newValue && !oldValue && !control.IsFocused)
            {
                control.Focus();
            }
        }

        #endregion
    }
}
