namespace Common.UnitTests
{
    using System;

    using NUnit.Framework;

    public static class AssertEx
    {
        public static T ThrowsOfType<T>(TestDelegate code, string message, params object[] args) where T : Exception
        {
            Exception caughtException = null;
            
            try
            {
                code();
            }
            catch (Exception exception)
            {
                caughtException = exception;
            }

            Assert.IsInstanceOf<T>(caughtException, message, args);
            return (T)caughtException;
        }
    }
}
