namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class TreeToTreeImageConverter : ImageConverterBase
    {
        protected override string GetCachePrefix()
        {
            return "TreeImage";
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string data)
            {
                return null;
            }

            BitmapImage image = GetImage(data);
            if (image != null)
            {
                return image;
            }

            byte[] bytes = MagicDatabase.GetTreePicture(data)?.Image;
            if (null != bytes && bytes.Length > 0)
            {
                return BytesToSvgImage(bytes, data);
            }

            return null;
        }
    }
}
