
namespace MagicPictureSetDownloader.Core.IO
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using MagicPictureSetDownloader.Interface;

    public static class ImportExportFormatterFactory
    {
        private static readonly IImportExportFormatter[] _formatters;

        static ImportExportFormatterFactory()
        {
           _formatters = Assembly.GetExecutingAssembly().GetTypes()
                                                        .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IImportExportFormatter)))
                                                        .Select(Activator.CreateInstance)
                                                        .Cast<IImportExportFormatter>()
                                                        .ToArray();
        }

        public static IImportExportFormatter Create(ExportFormat format)
        {
            return _formatters.FirstOrDefault(f => f.Format == format);
        }

        public static IImportExportFormatter CreateForFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string firstLine = sr.ReadLine();
                return _formatters.FirstOrDefault(f => f.IsMatchingPattern(firstLine));
            }
        }
    }
}
