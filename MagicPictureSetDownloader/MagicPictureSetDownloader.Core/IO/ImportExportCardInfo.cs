
namespace MagicPictureSetDownloader.Core.IO
{
    using MagicPictureSetDownloader.Interface;

    internal class ImportExportCardInfo : IImportExportCardCount
    {
        internal ImportExportCardInfo(int idGatherer, int number, int foilNumber, int idLanguage = 0)
        {
            IdGatherer = idGatherer;
            Number = number;
            FoilNumber = foilNumber;
            IdLanguage = idLanguage;
        }

        public int IdGatherer { get; private set; }
        public int Number { get; private set; }
        public int FoilNumber { get; private set; }
        public int IdLanguage { get; private set; }


        internal void Add(int number)
        {
            Number += number;
        }
        internal void AddFoil(int foilNumber)
        {
            FoilNumber += foilNumber;
        }

    }
}
