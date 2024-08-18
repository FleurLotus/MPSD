namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PriceImporterException : ApplicationException
    {
        public PriceImporterException()
        {
        }
        public PriceImporterException(string message, params object[] parameters)
            : base(string.Format(message, parameters))
        {
        }

        public PriceImporterException(string message)
            : base(message)
        {
        }
        public PriceImporterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public PriceImporterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
