namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    [ValueConversion(typeof(int), typeof(BitmapImage))]
    public class GathererIdToImageConverter : ImageConverterBase
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

            int idGatherer = System.Convert.ToInt32(value);

            if (idGatherer == 0)
            {
                return null;
            }

            BitmapImage image = GetImage(idGatherer.ToString());
            if (image != null)
            {
                return image;
            }

            byte[] bytes = MagicDatabase.GetPicture(idGatherer)?.Image;
            if (null != bytes && bytes.Length != 0)
            {
                return BytesToImage(bytes, idGatherer.ToString());
            }

            return GetDefaultCardImage();
        }
    }
}
