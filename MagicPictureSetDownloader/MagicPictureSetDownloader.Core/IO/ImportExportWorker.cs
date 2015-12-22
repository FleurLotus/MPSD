
namespace MagicPictureSetDownloader.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using MagicPictureSetDownloader.Db;
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

            IMagicDatabaseReadOnly magicDatabase = MagicDatabaseManager.ReadOnly;

            foreach (string collectionName in collectionNames)
            {
                ICardCollection cardcollection = magicDatabase.GetCollection(collectionName);
                IEnumerable<ICardInCollectionCount> cardsInCollection = magicDatabase.GetCardCollection(cardcollection);

                if (cardsInCollection == null)
                    throw new ImportExportException("Can't find collection named {0}", collectionName);

                string filePath = Path.Combine(outpath, collectionName + formatter.Extension);

                try
                {
                    using (StreamWriter sw = new StreamWriter(filePath, false))
                    {
                        sw.Write(formatter.ToFile(cardsInCollection));
                    }
                }
                catch (ImportExportException)
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    throw;
                }
            }
        }
        public void ImportToNewColletion(string importFilePath, string newCollectionName)
        {
            IMagicDatabaseReadAndWriteCollection magicDatabaseCollection = MagicDatabaseManager.ReadAndWriteCollection;
            ICardCollection collection = magicDatabaseCollection.InsertNewCollection(newCollectionName);
            if (collection == null)
                throw new ArgumentException("Collection name already exists", "newCollectionName");

            ImportToColletion(importFilePath, collection);
        }

        public void ImportToExistingColletion(string importFilePath, string collectionToCompletName)
        {
            IMagicDatabaseReadAndWriteCardInCollection magicDatabaseCollection = MagicDatabaseManager.ReadAndWriteCardInCollection;
            ICardCollection collection = magicDatabaseCollection.GetCollection(collectionToCompletName);
            if (collection == null)
                throw new ArgumentException("Collection name doesn't exist", "collectionToCompletName");

            ImportToColletion(importFilePath, collection);
        }
        private void ImportToColletion(string importFilePath, ICardCollection collection)
        {
            IImportExportCardCount[] cardToImport = GetImport(importFilePath).ToArray();
            IMagicDatabaseReadAndWriteCardInCollection magicDatabase = MagicDatabaseManager.ReadAndWriteCardInCollection;

            foreach (IImportExportCardCount importExportCardCount in cardToImport)
            {
                magicDatabase.InsertOrUpdateCardInCollection(collection.Id, importExportCardCount.IdGatherer, importExportCardCount.IdLanguage, importExportCardCount.Number, importExportCardCount.FoilNumber);
            }
        }
        private IEnumerable<IImportExportCardCount> GetImport(string importFilePath)
        {
            if (!File.Exists(importFilePath))
            {
                throw new ArgumentException("import file doesn't exist", "importFilePath");
            }

            IImportExportFormatter formatter = ImportExportFormatterFactory.CreateForFile(importFilePath);
            if (formatter == null)
            {
                throw new ArgumentException("Can't find appropriate formatter for " + importFilePath, "importFilePath");
            }

            using (StreamReader sr = new StreamReader(importFilePath))
            {
                return formatter.Parse(sr.ReadToEnd());
            }
        }
    }
}
