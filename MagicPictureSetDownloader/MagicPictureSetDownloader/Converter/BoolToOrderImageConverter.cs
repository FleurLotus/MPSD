namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using Common.WPF.Converter;

    [ValueConversion(typeof(bool), typeof(BitmapSource))]
    public class BoolToOrderImageConverter : NoConvertBackConverter 
    {
        private static readonly BitmapToImageConverter _bitmapToImageConverter = new BitmapToImageConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bitmap bitmap = (bool)value ? Properties.Resources.Asc : Properties.Resources.Desc;

            return _bitmapToImageConverter.Convert(bitmap, targetType, parameter, culture);
        }
    }
}
