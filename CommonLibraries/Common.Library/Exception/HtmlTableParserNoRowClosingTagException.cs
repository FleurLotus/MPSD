namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class HtmlTableParserNoRowClosingTagException : HtmlTableParserExceptionBase
    {
        #region Constructors and Destructors
        public HtmlTableParserNoRowClosingTagException()
            : base("Can't find any close tag for row")
        {
        }
        #endregion
    }
}
