 namespace MagicPictureSetDownloader.Core.IO
{
    using MagicPictureSetDownloader.Interface;

    internal class ImportExportCardInfo : IImportExportCardCount
    {
        internal ImportExportCardInfo(int idGatherer, int number, int foilNumber, int altArtNumber, int foilAltArtNumber, int idLanguage)
        {
            IdGatherer = idGatherer;
            Number = number;
            FoilNumber = foilNumber;
            AltArtNumber = altArtNumber;
            FoilAltArtNumber = foilAltArtNumber;
            IdLanguage = idLanguage;
        }

        public int IdGatherer { get; }
        public int Number { get; private set; }
        public int FoilNumber { get; private set; }
        public int AltArtNumber { get; private set; }
        public int FoilAltArtNumber { get; private set; }
        public int IdLanguage { get; }

        internal void Add(int number)
        {
            Number += number;
        }
        internal void AddFoil(int foilNumber)
        {
            FoilNumber += foilNumber;
        }
        internal void AddAltArt(int altArtNumber)
        {
            AltArtNumber += altArtNumber;
        }
        internal void AddFoilAltArt(int foilAltArtNumber)
        {
            FoilAltArtNumber += foilAltArtNumber;
        }

    }
}
