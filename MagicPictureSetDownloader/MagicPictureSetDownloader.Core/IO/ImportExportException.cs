namespace MagicPictureSetDownloader.Core.IO
{
    using System;

    [Serializable]
    public class ImportExportException : ApplicationException
    {
        public ImportExportException()
        {
        }
        public ImportExportException(string message, params object[] parameters)
            : base(string.Format(message, parameters))
        {
        }

        public ImportExportException(string message)
            : base(message)
        {
        }
        public ImportExportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}