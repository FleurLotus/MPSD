namespace Common.Database
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class AttributedTypeException : ApplicationException
    {
        public Type Type { get; private set; }

        public AttributedTypeException(Type type)
        {
            Type = type;
        }
        public AttributedTypeException(Type type, string message)
            : base(message)
        {
            Type = type;
        }
        public AttributedTypeException(Type type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }
        public AttributedTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

