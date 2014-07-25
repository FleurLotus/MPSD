namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value || null == parameter)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value || null == parameter)
                return null;

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            return useValue ? Enum.Parse(targetType, targetValue) : null;
        }
    }
}
