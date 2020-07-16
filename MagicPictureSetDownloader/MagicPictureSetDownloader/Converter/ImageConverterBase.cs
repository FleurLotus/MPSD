namespace MagicPictureSetDownloader.Converter
{
    using System.IO;
    using System.Windows.Media.Imaging;
    using System.Runtime.Caching;
    using System.Collections.Specialized;

    using Common.WPF;
    using Common.WPF.Converter;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using System;

    public abstract class ImageConverterBase : NoConvertBackConverter
    {
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

        protected BitmapImage BytesToImage(byte[] bytes, string key)
        {
            MemoryStream stream = new MemoryStream(bytes);
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
