using System;
using System.Runtime.Serialization;

namespace Common.Database
{
    internal class ApplicationDbException : ApplicationException
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

