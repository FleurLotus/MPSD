namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using MagicPictureSetDownloader.Interface;

    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class StringToImageConverter : ImageConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string data = value as string;

            if (data == null)
                return null;

            ITreePicture treepicture = MagicDatabase.GetTreePicture(data.ToUpper());
            if (null != treepicture && treepicture.Image.Length > 0)
            {
                BitmapImage image = BytesToImage(treepicture.Image);
                return image;
            }

            return null;
        }
    }
}
