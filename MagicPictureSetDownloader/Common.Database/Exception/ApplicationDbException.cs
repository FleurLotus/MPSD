namespace Common.Database
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ApplicationDbException : ApplicationException
    {
        public ApplicationDbException()
        {
        }
        public ApplicationDbException(string message)
            : base(message)
        {
        }
        public ApplicationDbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ApplicationDbException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

