namespace MagicPictureSetDownloader.DbGenerator
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Common.Zip;

    internal class Generator
    {
        internal Generator()
        {
        }
        internal string Generate(bool tempDir = false)
        {
            string resourceName = DatabaseGenerator.GetResourceName();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string outDir = tempDir ? Path.GetTempPath() : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string name = executingAssembly.GetManifestResourceNames().First(s => s.EndsWith(resourceName.Replace(".sqlite", ".zip")));

            using (Stream stream = executingAssembly.GetManifestResourceStream(name))
            {
                Zipper.UnZipAll(stream, outDir);
                return outDir;
            }
        }
        internal string GeneratePictures(bool tempDir = false)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string outDir = tempDir ? Path.GetTempPath() : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string name = executingAssembly.GetManifestResourceNames().First(s => s.EndsWith("MagicPicture.zip"));

            using (Stream stream = executingAssembly.GetManifestResourceStream(name))
            {
                Zipper.UnZipAll(stream, outDir);
                return outDir;
            }
        }
    }
}
