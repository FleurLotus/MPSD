namespace Common.Log
{
    using System;
    using System.IO;

    using Microsoft.Extensions.Logging;

    public class FileLogger : ILogger
    {
        private readonly string _path;
#if NET9_0_OR_GREATER
        private readonly System.Threading.Lock _lock = new System.Threading.Lock();
#else
        private readonly object _lock = new object();
#endif

        public FileLogger(string path)
        {
            _path = path;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            formatter ??= (s, e) => s == null ? e?.ToString() : s.ToString();

            string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)} {exception?.ToString()}";

            lock (_lock) // Ensure thread safety when writing to the file
            {
                File.AppendAllText(_path, message + Environment.NewLine);
            }
        }
    }
}
