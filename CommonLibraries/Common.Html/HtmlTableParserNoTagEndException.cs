namespace Common.Html
{
    using System;

    [Serializable]
    public class HtmlTableParserNoTagEndException : HtmlTableParserExceptionBase
    {
        #region Constructors and Destructors
        public HtmlTableParserNoTagEndException()
            : base("Invalid tag end")
        {
        }
        #endregion
    }
}
