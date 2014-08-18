namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Data;

    public class BoolToImageConverter : IValueConverter
    {
        private static readonly BitmapToImageConverter _bitmapToImageConverter = new BitmapToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bitmap bitmap = (bool)value ? Properties.Resources.Asc : Properties.Resources.Desc;

            return _bitmapToImageConverter.Convert(bitmap, targetType, parameter, culture);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
