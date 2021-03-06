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
            string data = value as string;

            if (data == null)
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
                return BytesToImage(bytes, data);
            }

            return null;
        }
    }
}
