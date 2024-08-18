namespace MagicPictureSetDownloader.ScryFall
{
    using System.Globalization;
    using System.IO;
    using System;

    internal static class FileManager
    {
        public static string GetSetFile()
        {
            return GetLastFile("sets-*.json");
        }
        public static string GetDefaultCardFile()
        {
            return GetLastFile("default-cards-*.json");
        }
        public static string GetAllCardFile()
        {
            return GetLastFile("all-cards-*.json");
        }
        private static string GetLastFile(string pattern)
        {
            DateTime lastdate = DateTime.MinValue;
            string resultFilePath = null;

            foreach (string filePath in Directory.GetFiles(".", pattern))
            {
                string temp = Path.GetFileNameWithoutExtension(filePath);
                if (temp.Length >= 14)
                {
                    temp = temp.Substring(temp.Length - 14);
                    if (DateTime.TryParseExact(temp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    {
                        if (lastdate < dt)
                        {
                            lastdate = dt;
                            resultFilePath = filePath;
                        }
                    }
                }
            }
            return resultFilePath;
        }
        public static string ToSmallName(string filePath)
        {
            return Path.Combine(Path.GetDirectoryName(filePath), "small-" + Path.GetFileName(filePath));
        }
    }
}
