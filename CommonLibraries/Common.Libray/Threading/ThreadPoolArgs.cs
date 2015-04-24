
namespace Common.Libray.Threading
{
    using System.Threading;

    public class ThreadPoolArgs
    {
        private readonly WaitCallback _callback;
        private readonly object _arg;

        public ThreadPoolArgs(WaitCallback callback, object arg)
        {
            _callback = callback;
            _arg = arg;
        }
        public void Invoke()
        {
            _callback(_arg);
        }
    }
}
