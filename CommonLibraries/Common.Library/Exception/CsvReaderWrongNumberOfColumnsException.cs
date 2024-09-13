namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class CsvReaderWrongNumberOfColumnsException : CsvReaderExceptionBase
    {
        #region Constructors and Destructors
        public CsvReaderWrongNumberOfColumnsException(int line, int found, int expectedColumn)
            : base($"At line {line}, found {found} column(s) while expected {expectedColumn}")
        {
        }
        #endregion
    }
}
