namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class HtmlTableParserNoTableClosingTagException : HtmlTableParserExceptionBase
    {
        #region Constructors and Destructors
        public HtmlTableParserNoTableClosingTagException()
            : base("Can't find any close tag from table")
        {
        }
        #endregion
    }
}
