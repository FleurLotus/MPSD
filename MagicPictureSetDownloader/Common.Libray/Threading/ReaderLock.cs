namespace Common.Libray.Threading
{
    using System;
    using System.Threading;

    public class ReaderLock : IDisposable
    {
        private bool _disposed;
        private readonly ReaderWriterLockSlim _readerWriter;

        public ReaderLock(ReaderWriterLockSlim readerWriter)
        {
            _readerWriter = readerWriter;
            _readerWriter.EnterReadLock();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _readerWriter.ExitReadLock();
            }
            _disposed = true;
        }


    }
}
