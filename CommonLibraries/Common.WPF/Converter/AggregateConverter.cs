namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class AggregateConverter : NoConvertBackConverter
    {
        private readonly IValueConverter[] _converters;

        public AggregateConverter(IValueConverter converter1, IValueConverter converter2)
        {
            _converters = new[] { converter1, converter2 };
        }
        public AggregateConverter(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3)
        {
            _converters = new[] { converter1, converter2, converter3 };
        }
        public AggregateConverter(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3, IValueConverter converter4)
        {
            _converters = new[] { converter1, converter2, converter3, converter4 };
        }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converters.Aggregate(value, (current, valueConverter) => valueConverter.Convert(current, targetType, parameter, culture));
        }
    }
}
