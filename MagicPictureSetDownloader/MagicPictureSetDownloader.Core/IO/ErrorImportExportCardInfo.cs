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

        public string SourceLine { get; private set; }
        public string ErrorMessage { get; private set; }

        public int IdGatherer
        {
            get { return -1; }
        }
        public int Number
        {
            get { return -1; }
        }
        public int FoilNumber
        {
            get { return -1; }
        }
        public int IdLanguage
        {
            get { return -1; }
        }
    }
}
