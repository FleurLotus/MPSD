namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ParserException : ApplicationException
    {
        public ParserException()
        {
        }
        public ParserException(string message) : base(message)
        {
        }
        public ParserException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public ParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
