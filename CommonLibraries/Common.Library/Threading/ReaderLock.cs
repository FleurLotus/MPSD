namespace Common.Library.Threading
{
    using System;
    using System.Threading;

    public class ReaderLock : IDisposable
    {
        private bool _disposed;
        private readonly ReaderWriterLockSlim _readerWriter;

        public ReaderLock(ReaderWriterLockSlim readerWriter)
        {
            if (readerWriter == null)
            {
                throw new ArgumentNullException(nameof(readerWriter));
            }

            _readerWriter = readerWriter;
            _readerWriter.EnterReadLock();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _readerWriter.ExitReadLock();
            }
            _disposed = true;
        }
    }
}
