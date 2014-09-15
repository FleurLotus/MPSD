namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;

    using MagicPictureSetDownloader.Interface;

    public class TreeToImageConverter : ImageConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string data = value as string;

            if (data == null)
                return null;

            ITreePicture treepicture = MagicDatabase.GetTreePicture(data);
            if (null != treepicture && treepicture.Image.Length > 0)
            {
                System.Windows.Media.Imaging.BitmapImage image =BytesToImage(treepicture.Image);
                return image;
            }

            return null;
        }
    }
}
