namespace Common.CSV
{
    using System;

    [Serializable]
    public class CsvReaderUnclosedEscapeException : CsvReaderExceptionBase
    {
        #region Constructors and Destructors
        public CsvReaderUnclosedEscapeException()
            : base("Stream ends while in escape reading field")
        {
        }
        #endregion
    }
}
