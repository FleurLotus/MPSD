﻿namespace Common.WPF
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    public class AutoComplete
    {
        // Based on http://blogs.developpeur.org/tom/archive/2009/02/18/wpf-comment-cr-er-une-combobox-qui-s-autocompl-te.aspx
        // Using a DependencyProperty as the backing store for Enabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(AutoComplete), new UIPropertyMetadata(false, EnabledValueChanged));
        public static readonly DependencyProperty CaseInsensitiveProperty = DependencyProperty.RegisterAttached("CaseInsensitive", typeof(bool), typeof(AutoComplete), new UIPropertyMetadata(false, CaseInsensitiveValueChanged));

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }
        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }
        public static bool GetCaseInsensitive(DependencyObject obj)
        {
            return (bool)obj.GetValue(CaseInsensitiveProperty);
        }
        public static void SetCaseInsensitive(DependencyObject obj, bool value)
        {
            obj.SetValue(CaseInsensitiveProperty, value);
        }

        private static void EnabledValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                if (combo.Template != null)
                {
                    SetTextChangedHandler(combo);
                }
                else
                {
                    combo.Loaded += ComboLoaded;
                }
            }
        }
        private static void ComboLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo == null)
            {
                return;
            }

            combo.Loaded -= ComboLoaded;
            if (combo.Template != null)
            {
                SetTextChangedHandler(combo);
            }
        }

        private static void CaseInsensitiveValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo != null && combo.Template != null && GetEnabled(combo))
            {
                TextBoxTextChanged(combo, new RoutedEventArgs());
            }
        }

        private static void SetTextChangedHandler(ComboBox combo)
        {
            TextBox textBox = combo.Template.FindName("PART_EditableTextBox", combo) as TextBox;
            if (textBox != null)
            {
                bool enabled = GetEnabled(combo);
                if (enabled)
                {
                    textBox.TextChanged += TextBoxTextChanged;
                }
                else
                {
                    textBox.TextChanged -= TextBoxTextChanged;
                }
            }
        }

        private static void TextBoxTextChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            ComboBox combo = textBox.TemplatedParent as ComboBox;
            //combo.IsDropDownOpen = true;
            if (combo == null)
            {
                return;
            }
            if (combo.IsTextSearchEnabled && textBox.SelectionStart == 0 && !string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            string text = combo.IsTextSearchEnabled ? textBox.Text.Substring(0, textBox.SelectionStart) : textBox.Text;
            bool caseInsensitive = GetCaseInsensitive(combo);
            
            combo.Items.Filter = value => value.ToString().StartsWith(text, caseInsensitive, CultureInfo.InvariantCulture);
        }
    }
}
