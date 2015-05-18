namespace Common.Zip
{
    using System.IO;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    public static class Zipper
    {
        public static Stream UnZipOneFile(byte[] stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(stream, 0, stream.Length);
                return UnZipOneFile(ms);
            }
        }

        public static Stream UnZipOneFile(Stream stream)
        {
            ZipFile zipFile = null;
            Stream outStream = null;
            try
            {
                zipFile = new ZipFile(stream);

                foreach (ZipEntry entry in zipFile)
                {
                    if (!entry.IsFile)
                        continue;

                    byte[] buffer = new byte[4096];

                    Stream zstream = zipFile.GetInputStream(entry);

                    outStream = new MemoryStream(10 * 1024 * 1024);
                    StreamUtils.Copy(zstream, outStream, buffer);
                    outStream.Seek(0, SeekOrigin.Begin);
                    break; // process the only one entry
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

            return outStream;
        }

    }
}
