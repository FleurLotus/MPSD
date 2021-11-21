namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    [ValueConversion(typeof(byte[]), typeof(BitmapImage))]
    public class ByteArrayToImageConverter : ImageConverterBase
    {
        protected override string GetCachePrefix()
        {
            return "ByteImage";
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not byte[] data)
            {
                return null;
            }

            return BytesToImage(data, null);
        }
    }
}
