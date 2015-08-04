namespace Common.Library
{
    using System;

    public interface ISplashScreen : IDisposable
    {
        void Progress(int perCent);
        void ChangeDisplayText(string text);
    }
}