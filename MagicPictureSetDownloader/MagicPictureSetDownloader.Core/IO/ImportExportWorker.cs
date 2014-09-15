
namespace MagicPictureSetDownloader.Core.IO
{
    using System;
    using System.IO;

    using MagicPictureSetDownloader.Interface;

    public class ImportExportWorker
    {
        public void Export(string[] collectionNames, string outpath, ExportFormat exportFormatSelected)
        {
            if (!Directory.Exists(outpath))
                throw new ArgumentException("output path doesn't exist", "outpath");

            IImportExportFormatter formatter = ImportExportFormatterFactory.Create(exportFormatSelected);
            if (formatter == null)
                throw new ArgumentException("Can't find appropriate formatter for " + exportFormatSelected, "exportFormatSelected");

            foreach (string collectionName in collectionNames)
            {
                string filePath = Path.Combine(outpath, collectionName + formatter.Extension);
                using (StreamWriter sw = new StreamWriter(filePath, false))
                {
                    //ALERT: TO CODE Export
                }
            }
        }
        public void ImportToNewColletion(string importFilePath, string newCollectionName)
        {
            //ALERT: TO CODE ImportToNewColletion
        }
        public void ImportToExistingColletion(string importFilePath, string collectionToCompletName)
        {
            //ALERT: TO CODE ImportToExistingColletion
        }
    }
}
