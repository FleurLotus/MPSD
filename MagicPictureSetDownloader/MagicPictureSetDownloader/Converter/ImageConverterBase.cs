namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using Common.WPF;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public abstract class ImageConverterBase : IValueConverter
    {
        protected readonly IMagicDatabaseReadOnly MagicDatabase = Lib.IsInDesignMode() ? null : MagicDatabaseManager.ReadOnly;
        
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
