namespace MagicPictureSetDownloader.DbGenerator
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Common.Zip;

    internal class Generator
    {
        private readonly DatabaseType _data;

        internal Generator(DatabaseType data)
        {
            _data = data;
        }
        internal string Generate(bool tempDir = false)
        {
            string resourceName = DatabaseGenerator.GetResourceName(_data);
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string outDir = tempDir ? Path.GetTempPath() : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string name = executingAssembly.GetManifestResourceNames().First(s => s.EndsWith(resourceName.Replace(".sqlite", ".zip")));

            using (Stream stream = executingAssembly.GetManifestResourceStream(name))
            {
                Zipper.UnZipAll(stream, outDir);
                return outDir;
            }
        }
    }
}
