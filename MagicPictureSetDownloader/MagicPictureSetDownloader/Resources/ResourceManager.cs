
namespace MagicPictureSetDownloader.Resources
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    using Common.Library.Extension;

    public static class ResourceManager
    {
        private static readonly IDictionary<string, Bitmap> _images = new Dictionary<string, Bitmap>();

        static ResourceManager()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (string name in executingAssembly.GetManifestResourceNames().Where(n => n.EndsWith(".png")))
            {
                // ReSharper disable AssignNullToNotNullAttribute
                Bitmap bitmap = new Bitmap(executingAssembly.GetManifestResourceStream(name));
                // ReSharper restore AssignNullToNotNullAttribute
                string key = name.Substring(0, name.Length - 4);
                key = key.Substring(key.LastIndexOf('.') + 1);
                _images.Add(key, bitmap);
            }
        }

        private static Bitmap GetResource(string name)
        {
            return _images.GetOrDefault(name);
        }

        public static Bitmap Asc
        {
            get { return GetResource("Asc"); }
        }
        public static Bitmap Desc
        {
            get { return GetResource("Desc"); }
        }
        public static Bitmap Down
        {
            get { return GetResource("Down"); }
        }
        public static Bitmap Icon
        {
            get { return GetResource("Icon"); }
        }
        public static Bitmap Up
        {
            get { return GetResource("Up"); }
        }
    }
}
