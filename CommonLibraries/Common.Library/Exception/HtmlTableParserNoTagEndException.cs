namespace Common.Library.Exception
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
