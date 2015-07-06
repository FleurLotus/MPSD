namespace Common.Libray.Threading
{
    using System;
    using System.Threading;

    public class UpgradeableReadLock : IDisposable
    {
        private bool _disposed;
        private readonly ReaderWriterLockSlim _readerWriter;

        public UpgradeableReadLock(ReaderWriterLockSlim readerWriter)
        {
            if (readerWriter == null)
                throw new ArgumentNullException("readerWriter");

            _readerWriter = readerWriter;
            _readerWriter.EnterUpgradeableReadLock();
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
                _readerWriter.ExitUpgradeableReadLock();
            }
            _disposed = true;
        }
    }
}
