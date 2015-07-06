namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class HandlerAlreadyKnownException : ApplicationException
    {
        #region Constructors and Destructors

        public HandlerAlreadyKnownException()
            : base("Handler already added")
        {
        }

        #endregion
    }
}

