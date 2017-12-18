namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class CsvReaderErrorStateException : CsvReaderExceptionBase
    {
        #region Constructors and Destructors
        public CsvReaderErrorStateException()
            : base("CsvReader is in error state, can't read anymore")
        {
        }
        #endregion
    }
}
