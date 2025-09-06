namespace Common.CSV
{
    using System;

    [Serializable]
    public class CsvReaderBadClosingEscapeException : CsvReaderExceptionBase
    {
        #region Constructors and Destructors
        public CsvReaderBadClosingEscapeException()
            : base("Escape must start and end the field")
        {
        }
        #endregion
    }
}
