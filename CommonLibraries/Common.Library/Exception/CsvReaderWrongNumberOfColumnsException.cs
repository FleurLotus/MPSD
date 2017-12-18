namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class CsvReaderWrongNumberOfColumnsException : CsvReaderExceptionBase
    {
        #region Constructors and Destructors
        public CsvReaderWrongNumberOfColumnsException(int line, int found, int expectedColumn)
            : base(string.Format("At line {0}, found {1} column(s) while expected {2}", line, found, expectedColumn))
        {
        }
        #endregion
    }
}
