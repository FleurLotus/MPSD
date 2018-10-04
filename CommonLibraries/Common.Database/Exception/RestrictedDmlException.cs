namespace Common.Database
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class RestrictedDmlException : ApplicationException
    {
        public string TableName { get; }
        public Restriction Restriction { get; }

        public RestrictedDmlException(string tableName, Restriction restriction)
        {
            TableName = tableName;
            Restriction = restriction;
        }
        public RestrictedDmlException(string tableName, Restriction restriction, string message)
            : base(message)
        {
            TableName = tableName;
            Restriction = restriction;
        }
        public RestrictedDmlException(string tableName, Restriction restriction, string message, Exception innerException)
            : base(message, innerException)
        {
            TableName = tableName;
            Restriction = restriction;
        }
        public RestrictedDmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
