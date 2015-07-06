namespace Common.Library.Exception
{
    using System;

    [Serializable]
    public class HandlerNotKnownException : ApplicationException
    {
        #region Constructors and Destructors

        public HandlerNotKnownException()
            : base("Handler not known or already removed")
        {
        }

        #endregion
    }
}

