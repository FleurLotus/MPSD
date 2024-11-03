namespace Common.Database
{
    using System;

    [Serializable]
    internal class AttributedTypeException : ApplicationException
    {
        public Type Type { get; }

        public AttributedTypeException(Type type, string message)
            : base(message)
        {
            Type = type;
        }
    }
}
