namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Audit : IAudit
    {
        public Audit()
        {
            OperationDate = DateTime.UtcNow;
        }

        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public DateTime OperationDate { get; set; }
        [DbColumn]
        public int IdCollection { get; set; }
        [DbColumn]
        public int? IdGatherer { get; set; }
        [DbColumn]
        public bool? IsFoil { get; set; }
        [DbColumn]
        public bool? IsAltArt { get; set; }
        [DbColumn]
        public int? IdLanguage { get; set; }
        [DbColumn]
        public int Quantity { get; set; }

        //IdGather, IsFoil, IdLanguage null or not are linked
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public override string ToString()
        {
            if (IdGatherer.HasValue)
            {
                return string.Format(" {0} card(s) {1} {2}{3}{4} to collection {5} at {6:yyyy-MM-dd HH:mm:ss.ff}", Quantity, 
                                                                                                                IdGatherer.Value,
                                                                                                                IsFoil.Value ? "(Foil)" : string.Empty, 
                                                                                                                IsAltArt.Value ? "(AltArt)" : string.Empty, 
                                                                                                                IdLanguage.Value, 
                                                                                                                IdCollection, 
                                                                                                                OperationDate);
            }

            return string.Format("{0} collection {1} at {2:yyyy-MM-dd HH:mm:ss.ff}", Quantity > 0 ? "Create" : "Delete", IdCollection, OperationDate);
        }
    }
}
