namespace Common.Html
{
    using System;

    [Serializable]
    public abstract class HtmlTableParserExceptionBase : ApplicationException
    {
        #region Constructors and Destructors
        protected HtmlTableParserExceptionBase(string message)
            : base(message)
        {
        }
        #endregion
    }
}