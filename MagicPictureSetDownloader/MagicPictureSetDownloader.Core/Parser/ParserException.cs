namespace MagicPictureSetDownloader.Core
{
    using System;

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
    }
}
