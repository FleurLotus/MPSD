namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof (object), typeof (bool))]
    public class ObjectNullToBoolConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }
    }
}
