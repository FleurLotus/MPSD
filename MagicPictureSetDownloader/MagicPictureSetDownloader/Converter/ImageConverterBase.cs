namespace MagicPictureSetDownloader.Converter
{
    using System.IO;
    using System.Windows.Media.Imaging;

    using Common.WPF;
    using Common.WPF.Converter;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public abstract class ImageConverterBase : NoConvertBackConverter
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
    }
}
