namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF.Converter;

    [ValueConversion(typeof(int), typeof(double))]
    public class ValueToPriceVisibleConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            return System.Convert.ToInt32(value) / 100.0;
        }
    }
}
