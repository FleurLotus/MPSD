
namespace MagicPictureSetDownloader.Db
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;

    using Common.Database;
    using Common.Libray.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.DbGenerator;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        public ICollection<IAudit> GetAllAudits()
        {
            using (new ReaderLock(_lock))
            using (DbConnection cnx = _databaseConnection.GetMagicConnection(DatabasebType.Data))
            {
                return Mapper<Audit>.LoadAll(cnx).Cast<IAudit>().ToArray();
            }
        }

        private void AuditAddCollection(int idCollection)
        {
            if (idCollection <= 0)
                return;

            InsertNewAudit(new Audit { IdCollection = idCollection, Quantity = 1});
        }
        private void AuditRemoveCollection(int idCollection)
        {
            if (idCollection <= 0)
                return;

            InsertNewAudit(new Audit { IdCollection = idCollection, Quantity = -1});
        }
        private void AuditAddCard(int idCollection, int idGatherer, int idLanguage, bool isFoil, int countToAdd)
        {
            if (idCollection <= 0 || countToAdd == 0 || idGatherer <= 0 || idLanguage < 0)
                return;

            InsertNewAudit(new Audit
                               {
                                   IdCollection = idCollection,
                                   Quantity = countToAdd,
                                   IdGatherer = idGatherer,
                                   IsFoil = isFoil,
                                   IdLanguage = idLanguage
                               });
        }

        private void InsertNewAudit(Audit audit)
        {
            if (audit == null)
                return;
            using (new WriterLock(_lock))
            {
                using (DbConnection cnx = _databaseConnection.GetMagicConnection(DatabasebType.Data))
                {
                    Mapper<Audit>.InsertOne(cnx, audit);
                }
            }
        }
    }
}
