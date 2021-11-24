namespace Common.Zip
{
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip;

    public static class Zipper
    {
        public static void UnZipAll(byte[] stream, string outputDirectory, bool overrideExisting = false)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(stream, 0, stream.Length);
                UnZipAll(ms, outputDirectory, overrideExisting);
            }
        }
        public static void UnZipAll(Stream stream, string outputDirectory, bool overrideExisting = false)
        {
            ZipFile zipFile = null;
            try
            {
                zipFile = new ZipFile(stream);

                foreach (ZipEntry entry in zipFile)
                {
                    if (!entry.IsFile)
                    {
                        continue;
                    }

                    Stream zstream = zipFile.GetInputStream(entry);

                    string filePath = Path.Combine(outputDirectory, entry.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    if (!File.Exists(filePath) || overrideExisting)
                    {
                        using (FileStream sw = new FileStream(filePath, FileMode.Create))
                        {
                            zstream.CopyTo(sw);
                        }
                    }
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
