namespace Common.Database
{
    using System;

    [Serializable]
    public class ApplicationDbException : ApplicationException
    {
        public ApplicationDbException(string message)
            : base(message)
        {
        }
    }
}