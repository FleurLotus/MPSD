namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class CsvReaderNoReadCallException : CsvReaderExceptionBase
    {
        #region Constructors and Destructors
        public CsvReaderNoReadCallException()
            : base("Call read before try to access data")
        {
        }
        #endregion
    }
}

