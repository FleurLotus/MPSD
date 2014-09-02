namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using MagicPictureSetDownloader.Core;

    public abstract class ImageConverterBase : IValueConverter
    {
        protected readonly MagicDatabaseManager MagicDatabaseManager = new MagicDatabaseManager();
        
        protected static BitmapImage BytesToImage(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}