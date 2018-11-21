namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool) value)? Visibility.Visible: Visibility.Collapsed;
        }
    }
}
