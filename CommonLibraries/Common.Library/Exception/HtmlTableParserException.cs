namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class HtmlTableParserException : ApplicationException
    {
        #region Constructors and Destructors

        public HtmlTableParserException(string message)
            : base(message)
        {
        }
        #endregion
    }
}

