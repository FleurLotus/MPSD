 namespace MagicPictureSetDownloader.Core.IO
{
    using MagicPictureSetDownloader.Interface;

    internal class ErrorImportExportCardInfo : IImportExportCardCount
    {
        internal ErrorImportExportCardInfo(string line, string errorMessage)
        {
            SourceLine = line;
            ErrorMessage = errorMessage;
        }

        public string SourceLine { get; }
        public string ErrorMessage { get; }

        public string IdScryFall
        {
            get { return null; }
        }
        public int Number
        {
            get { return -1; }
        }
        public int FoilNumber
        {
            get { return -1; }
        }
        public int AltArtNumber
        {
            get { return -1; }
        }
        public int FoilAltArtNumber
        {
            get { return -1; }
        }
        public int IdLanguage
        {
            get { return -1; }
        }

        public int GetCount(ICardCountKey key)
        {
            return -1;
        }
        public ICardCount GetCardCount()
        {
            return null;
        }
    }
}
