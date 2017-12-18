namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class HtmlTableParserMultiTableException : HtmlTableParserExceptionBase
    {
        #region Constructors and Destructors
        public HtmlTableParserMultiTableException()
            : base("Multi table in the input text")
        {
        }
        #endregion
    }
}
