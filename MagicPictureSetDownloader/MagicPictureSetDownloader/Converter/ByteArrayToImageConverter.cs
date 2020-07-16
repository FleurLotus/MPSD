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
            byte[] data = value as byte[];

            if (data == null)
            {
                return null;
            }

            return BytesToImage(data, null);
        }
    }
}
