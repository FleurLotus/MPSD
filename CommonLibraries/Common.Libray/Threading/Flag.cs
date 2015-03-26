
namespace Common.Libray.Threading
{
    using System;

    internal sealed class Flag : IDisposable
    {
        private readonly object _sync = new object();
        private bool _disposed;
        
        private readonly FlagCount _flagCount;
        private readonly object _source;

        internal Flag(FlagCount flagCount, object source)
        {
            _flagCount = flagCount;
            _source = source;
            _flagCount.Increment(_source);
        }

        public void Dispose()
        {
            lock (_sync)
            {
                if (_disposed)
                    throw new ObjectDisposedException("Flag");

                _flagCount.Decrement(_source);

                _disposed = true;
            }
        }
    }
}
