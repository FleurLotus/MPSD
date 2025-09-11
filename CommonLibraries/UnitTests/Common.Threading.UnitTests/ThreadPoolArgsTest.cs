namespace Common.Threading.UnitTests
{
    using Common.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class ThreadPoolArgsTest
    {
        private object _param;

        [Test]
        public void TestInvoke()
        {
            object o = new object();
            ThreadPoolArgs threadPoolArgs = new ThreadPoolArgs(Callback, o);
            Assert.That(_param, Is.Null);
            threadPoolArgs.Invoke();
            Assert.That(_param, Is.EqualTo(o));
        }
        private void Callback(object o)
        {
            _param = o;
        }
    }
}