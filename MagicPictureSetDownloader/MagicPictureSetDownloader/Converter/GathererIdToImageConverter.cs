namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    using MagicPictureSetDownloader.Interface;

    [ValueConversion(typeof(int), typeof(IList<BitmapImage>))]
    public class GathererIdToImageConverter : ImageConverterBase
    {
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

            IPicture picture = MagicDatabase.GetPicture(idGatherer);
            if (null == picture || picture.Image == null || picture.Image.Length == 0)
            {
                picture = MagicDatabase.GetDefaultPicture();
            }

            if (null == picture || picture.Image == null || picture.Image.Length == 0)
            {
                return null;
            }

            return BytesToImage(picture.Image);
        }
    }
}
