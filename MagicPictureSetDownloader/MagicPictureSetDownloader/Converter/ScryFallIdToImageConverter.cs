namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    [ValueConversion(typeof(int), typeof(BitmapImage))]
    public class ScryFallIdToImageConverter : ImageConverterBase
    {
        protected override string GetCachePrefix()
        {
            return "CardImage";
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string idScryFall = (string)value;

            if (string.IsNullOrEmpty(idScryFall))
            {
                return null;
            }

            BitmapImage image = GetImage(idScryFall);
            if (image != null)
            {
                return image;
            }

            byte[] bytes = MagicDatabase.GetPicture(idScryFall)?.Image;
            if (null != bytes && bytes.Length != 0)
            {
                return BytesToImage(bytes, idScryFall);
            }

            if (int.Parse(parameter.ToString()) != 0)
            {
                return null;
            }

            return GetDefaultCardImage();
        }
    }
}
