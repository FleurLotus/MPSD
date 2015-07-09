namespace Common.Library.CSV
{
    using System;

    public interface ICsvReader: IDisposable
    {
        bool WithHeader { get; }
        char Separator { get; }
        bool EndOfStream { get; }

        int GetCurrentLine();
        int GetColumnsCount();
        string[] GetHeaders();
        string GetValue(int index);
        bool Read();
    }
}