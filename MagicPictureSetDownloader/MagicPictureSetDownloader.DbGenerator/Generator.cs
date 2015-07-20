namespace MagicPictureSetDownloader.DbGenerator
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Common.Zip;

    internal class Generator
    {
        private readonly DatabasebType _data;

        internal Generator(DatabasebType data)
        {
            _data = data;
        }
        internal void Generate()
        {
            string resourceName = DatabaseGenerator.GetResourceName(_data);
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            string name = executingAssembly.GetManifestResourceNames().First(s => s.EndsWith(resourceName.Replace(".sqlite", ".zip")));

            using (Stream stream = executingAssembly.GetManifestResourceStream(name))
                Zipper.UnZipAll(stream, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }
    }
}
