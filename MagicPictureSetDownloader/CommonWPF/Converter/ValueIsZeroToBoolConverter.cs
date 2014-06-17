using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace CommonWPF.Converter
{
    [ValueConversion(typeof (ICollection), typeof (bool))]
    public class ValueIsZeroToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
