namespace MagicPictureSetDownloader.Core.IO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using MagicPictureSetDownloader.Interface;

    public class ImportStatus
    {
        private ImportStatus()
        {
        }

        public int TotalCard { get; private set; }
        public int TotalKoLine { get; private set; }
        public string RebuiltErrorFile { get; private set; }
        public string ErrorMessage { get; private set; }
        public IEnumerable<IImportExportCardCount> ReadyToBeInserted { get; private set; }

        internal static ImportStatus BuildStatus(IEnumerable<IImportExportCardCount> cardsImport)
        {
            IList<IImportExportCardCount> list = new List<IImportExportCardCount>();

            int totalCard = 0;
            int totalKoLine = 0;

            StringBuilder sbFile = new StringBuilder();
            StringBuilder sbErrorMessage = new StringBuilder();


            foreach (IImportExportCardCount importExportCardCount in cardsImport)
            {
                ImportExportCardInfo okCard = importExportCardCount as ImportExportCardInfo;
                if (okCard != null)
                {
                    list.Add(okCard);
                    totalCard += okCard.FoilNumber + okCard.Number;
                    continue;
                }

                ErrorImportExportCardInfo koCard = importExportCardCount as ErrorImportExportCardInfo;
                if (koCard != null)
                {
                    sbErrorMessage.AppendLine(string.Format("{0} -> {1}", koCard.SourceLine, koCard.ErrorMessage));
                    sbFile.AppendLine(koCard.SourceLine);

                    totalKoLine++;
                    continue;
                }

                throw new ParserException("Unknown result type");
            }

            return new ImportStatus
                       {
                           TotalCard = totalCard,
                           ReadyToBeInserted = list.ToArray(),
                           TotalKoLine = totalKoLine,
                           RebuiltErrorFile = sbFile.ToString(),
                           ErrorMessage = sbErrorMessage.ToString()
                       };
        }
    }
}
