namespace Common.Zip
{
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip;

    public static class Zipper
    {
        public static void UnZipAll(byte[] stream, string outputDirectory)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(stream, 0, stream.Length);
                UnZipAll(ms, outputDirectory);
            }
        }
        public static void UnZipAll(Stream stream, string outputDirectory)
        {
            ZipFile zipFile = null;
            try
            {
                zipFile = new ZipFile(stream);

                foreach (ZipEntry entry in zipFile)
                {
                    if (!entry.IsFile)
                        continue;

                    Stream zstream = zipFile.GetInputStream(entry);

                    string filePath = Path.Combine(outputDirectory, entry.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (FileStream sw = new FileStream(filePath, FileMode.CreateNew))
                        zstream.CopyTo(sw);
                }
            }
            finally
            {
                if (null != zipFile)
                {
                    zipFile.IsStreamOwner = true;
                    zipFile.Close();
                }
            }
        }
    }
}
