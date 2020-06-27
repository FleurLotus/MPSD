namespace MagicPictureSetDownloader.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class ImportExportWorker
    {
        public void Export(string[] collectionNames, string outpath, ExportFormat exportFormatSelected)
        {
            if (!Directory.Exists(outpath))
            {
                throw new ArgumentException("output path doesn't exist", "outpath");
            }

            IImportExportFormatter formatter = ImportExportFormatterFactory.Create(exportFormatSelected);
            if (formatter == null)
            {
                throw new ArgumentException("Can't find appropriate formatter for " + exportFormatSelected, "exportFormatSelected");
            }

            IMagicDatabaseReadOnly magicDatabase = MagicDatabaseManager.ReadOnly;

            foreach (string collectionName in collectionNames)
            {
                ICardCollection cardcollection = magicDatabase.GetCollection(collectionName);
                IEnumerable<ICardInCollectionCount> cardsInCollection = magicDatabase.GetCardCollection(cardcollection);

                if (cardsInCollection == null)
                {
                    throw new ImportExportException("Can't find collection named {0}", collectionName);
                }

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
                    {
                        File.Delete(filePath);
                    }

                    throw;
                }
            }
        }
        public ImportStatus ImportToNewCollection(string importFilePath, string newCollectionName)
        {
            IMagicDatabaseReadAndWriteCollection magicDatabaseCollection = MagicDatabaseManager.ReadAndWriteCollection;
            ICardCollection collection = magicDatabaseCollection.InsertNewCollection(newCollectionName);
            if (collection == null)
            {
                throw new ArgumentException("Collection name already exists", "newCollectionName");
            }

            return ImportToCollection(importFilePath, collection);
        }

        public ImportStatus ImportToExistingCollection(string importFilePath, string collectionToCompletName)
        {
            IMagicDatabaseReadAndWriteCardInCollection magicDatabaseCollection = MagicDatabaseManager.ReadAndWriteCardInCollection;
            ICardCollection collection = magicDatabaseCollection.GetCollection(collectionToCompletName);
            if (collection == null)
            {
                throw new ArgumentException("Collection name doesn't exist", "collectionToCompletName");
            }

            return ImportToCollection(importFilePath, collection);
        }
        private ImportStatus ImportToCollection(string importFilePath, ICardCollection collection)
        {
            ImportStatus status = ImportStatus.BuildStatus(GetImport(importFilePath));

            IMagicDatabaseReadAndWriteCardInCollection magicDatabase = MagicDatabaseManager.ReadAndWriteCardInCollection;

            using (magicDatabase.BatchMode())
            {
                //Add in database the good one
                foreach (IImportExportCardCount importExportCardCount in status.ReadyToBeInserted)
                {
                    magicDatabase.InsertOrUpdateCardInCollection(collection.Id, importExportCardCount.IdGatherer, importExportCardCount.IdLanguage, importExportCardCount.Number, importExportCardCount.FoilNumber, importExportCardCount.AltArtNumber, importExportCardCount.FoilAltArtNumber);
                }
            }

            return status;
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
