namespace Common.Threading
{
    using System;

    internal sealed class Flag : IDisposable
    {
#if NET9_0_OR_GREATER
        private readonly System.Threading.Lock _sync = new System.Threading.Lock();
#else
        private readonly object _sync = new object();
#endif
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
#if NET7_0_OR_GREATER
                ObjectDisposedException.ThrowIf(_disposed, this);
#else
                if (_disposed)
                {
                    throw new ObjectDisposedException("Flag");
                }
#endif
                _flagCount.Decrement(_source);

                _disposed = true;
            }
        }
    }
}