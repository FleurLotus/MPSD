﻿ namespace MagicPictureSetDownloader.Db
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Common.Database;
    using Common.Library.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.DbGenerator;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        public ICollection<IAudit> GetAllAudits()
        {
            using (new ReaderLock(_lock))
            using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
            {
                return Mapper<Audit>.LoadAll(cnx).Cast<IAudit>().ToArray();
            }
        }

        private void AuditAddCollection(int idCollection)
        {
            if (idCollection <= 0)
            {
                return;
            }

            InsertNewAudit(new Audit { IdCollection = idCollection, Quantity = 1});
        }
        private void AuditRemoveCollection(int idCollection)
        {
            if (idCollection <= 0)
            {
                return;
            }

            InsertNewAudit(new Audit { IdCollection = idCollection, Quantity = -1});
        }
        private void AuditAddCard(int idCollection, int idGatherer, int idLanguage, ICardCount cardCount)
        {
            foreach (KeyValuePair<ICardCountKey, int> kv in cardCount)
            {
                AuditAddCard(idCollection, idGatherer, idLanguage, kv.Key, kv.Value);
            }
        }
        private void AuditAddCard(int idCollection, int idGatherer, int idLanguage, ICardCountKey cardCountKey, int countToAdd)
        {
            if (idCollection <= 0 || countToAdd == 0 || idGatherer == 0 || idLanguage < 0 || cardCountKey == null)
            {
                return;
            }

            InsertNewAudit(new Audit
                               {
                                   IdCollection = idCollection,
                                   Quantity = countToAdd,
                                   IdGatherer = idGatherer,
                                   IsFoil = cardCountKey.IsFoil,
                                   IsAltArt = cardCountKey.IsAltArt,
                                   IdLanguage = idLanguage
                               });
        }

        private void InsertNewAudit(Audit audit)
        {
            if (audit == null)
            {
                return;
            }

            using (new WriterLock(_lock))
            {
                using (IDbConnection cnx = _databaseConnection.GetMagicConnection(DatabaseType.Data))
                {
                    Mapper<Audit>.InsertOne(cnx, audit);
                }
            }
        }
    }
}
