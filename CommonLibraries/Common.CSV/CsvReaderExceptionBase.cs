namespace Common.CSV
{
    using System;

    [Serializable]
    public abstract class CsvReaderExceptionBase : ApplicationException
    {
        #region Constructors and Destructors
        protected CsvReaderExceptionBase(string message)
            : base(message)
        {
        }
        #endregion
    }
}
