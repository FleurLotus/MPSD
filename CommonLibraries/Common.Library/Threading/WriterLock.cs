namespace Common.Library.Threading
{
    using System;
    using System.Threading;

    public class WriterLock : IDisposable
    {
        private bool _disposed;
        private readonly ReaderWriterLockSlim _readerWriter;

        public WriterLock(ReaderWriterLockSlim readerWriter)
        {
            if (readerWriter == null)
                throw new ArgumentNullException("readerWriter");

            _readerWriter = readerWriter;
            _readerWriter.EnterWriteLock();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _readerWriter.ExitWriteLock();
            }
            _disposed = true;
        }
    }
}
