namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public abstract class NoConvertBackConverter : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
