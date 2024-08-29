namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;
    using System.Runtime.Caching;
    using System.Collections.Specialized;

    using SharpVectors.Converters;
    using SharpVectors.Renderers.Wpf;

    using Common.WPF;
    using Common.WPF.Converter;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public abstract class ImageConverterBase : NoConvertBackConverter
    {
        private static StreamSvgConverter StreamSvgConverter = new StreamSvgConverter(new WpfDrawingSettings { IncludeRuntime = false, TextAsGeometry = false });
        private const string DefaultCardImage = "Default";

        private static readonly MemoryCache cache = new MemoryCache("Cache", new NameValueCollection() { { "CacheMemoryLimitMegabytes", "100" } });

        protected readonly IMagicDatabaseReadOnly MagicDatabase = Lib.IsInDesignMode() ? null : MagicDatabaseManager.ReadOnly;

        protected abstract string GetCachePrefix();

        private string GetCacheKey(string key)
        {
            return GetCachePrefix() + key;
        }
        protected BitmapImage GetImage(string key)
        {
            return cache.Get(GetCacheKey(key)) as BitmapImage;
        }

        protected BitmapImage GetDefaultCardImage()
        {
            BitmapImage image = GetImage(DefaultCardImage);
            if (image != null)
            {
                return image;
            }

            byte[] bytes = MagicDatabase.GetDefaultPicture()?.Image;
            if (null != bytes && bytes.Length != 0)
            {
                return BytesToImage(bytes, DefaultCardImage);
            }

            return null;
        }
        protected BitmapImage BytesToSvgImage(byte[] bytes, string key)
        {
            MemoryStream svgStream = new MemoryStream(bytes);
            MemoryStream stream = new MemoryStream();
            StreamSvgConverter.Convert(svgStream, stream);
            
            return StreamToImage(stream, key);
        }
        protected BitmapImage BytesToImage(byte[] bytes, string key)
        {
            return StreamToImage(new MemoryStream(bytes), key);
        }

        private BitmapImage StreamToImage(Stream stream, string key)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            if (!string.IsNullOrEmpty(key))
            {
                cache.Add(GetCacheKey(key), image, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) });
            }

            return image;
        }
    }
}
