namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value || null == parameter)
                return false;

            string checkValue = value.ToString();
            string targetValues = parameter.ToString();
            return targetValues.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries)
                               .Any(targetValue => checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value || null == parameter)
                return null;

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            return useValue && !targetValue.Contains("@") ? Enum.Parse(targetType, targetValue) : null;
        }
    }
}
