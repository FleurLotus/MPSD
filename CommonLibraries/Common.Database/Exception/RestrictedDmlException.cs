namespace Common.Database
{
    using System;

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
    }
}
