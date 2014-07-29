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

            ITreePicture treepicture = MagicDatabaseManager.GetTreePicture(data);
            if (null != treepicture && treepicture.Image.Length > 0)
                return BytesToImage(treepicture.Image);

            return null;
        }
    }
}
