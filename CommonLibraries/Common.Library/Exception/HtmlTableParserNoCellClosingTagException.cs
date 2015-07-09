namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class HtmlTableParserNoCellClosingTagException : HtmlTableParserExceptionBase
    {
        #region Constructors and Destructors
        public HtmlTableParserNoCellClosingTagException()
            : base("Can't find any close tag for cells")
        {
        }
        #endregion
    }
}

