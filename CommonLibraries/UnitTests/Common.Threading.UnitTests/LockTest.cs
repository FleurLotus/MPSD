namespace Common.Threading.UnitTests
{
    using System;
    using System.Threading;

    using NUnit.Framework;

    using Common.Threading;

    [TestFixture]
    public class LockTest
    {
        private const int timeout = 30;

        [Test]
        public void TestReaderLockNullArgument()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("readerWriter"), () => new ReaderLock(null), "Null string arg should throw ArgumentNullException");
        }
        [Test]
        public void TestWriterLockNullArgument()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("readerWriter"), () => new WriterLock(null), "Null string arg should throw ArgumentNullException");
        }
        [Test]
        public void TestUpgradeableReadLockNullArgument()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("readerWriter"), () => new UpgradeableReadLock(null), "Null string arg should throw ArgumentNullException");
        }
        [Test]
        public void TestReaderLockMultiDispose()
        {
            ReaderLock mylock = new ReaderLock(new ReaderWriterLockSlim());
            mylock.Dispose();
            Assert.DoesNotThrow(() => mylock.Dispose());
        }
        [Test]
        public void TestWriterLockMultiDispose()
        {
            WriterLock mylock = new WriterLock(new ReaderWriterLockSlim());
            mylock.Dispose();
            Assert.DoesNotThrow(() => mylock.Dispose());
        }
        [Test]
        public void TestUpgradeableReadLockMultiDispose()
        {
            UpgradeableReadLock mylock = new UpgradeableReadLock(new ReaderWriterLockSlim());
            mylock.Dispose();
            Assert.DoesNotThrow(() => mylock.Dispose());
        }
        [Test]
        public void TestLockUpgradeable()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

            Assert.DoesNotThrow(() =>
            {
                using (new UpgradeableReadLock(lockSlim))
                {
                    using (new WriterLock(lockSlim))
                    {

                    }
                }
            });
        }
        [Test]
        public void TestLockReadvsRead()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new ReaderLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new ReaderLock(lockSlim))
            {
                Assert.That(finished, Is.False);
            }
        }
        [Test]
        public void TestLockReadvsWrite()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new ReaderLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new WriterLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
        [Test]
        public void TestLockWritevsWrite()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new WriterLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new WriterLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
        [Test]
        public void TestLockWritevsRead()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new WriterLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new ReaderLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
        [Test]
        public void TestLockUpgradeableNotPromotedvsRead()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new UpgradeableReadLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new ReaderLock(lockSlim))
            {
                Assert.That(finished, Is.False);
            }
        }
        [Test]
        public void TestLockReadvsUpgradeableNotPromoted()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new ReaderLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new UpgradeableReadLock(lockSlim))
            {
                Assert.That(finished, Is.False);
            }
        }
        [Test]
        public void TestLockUpgradeableNotPromotedvsWrite()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new UpgradeableReadLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new WriterLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
        [Test]
        public void TestLockWritevsUpgradeableNotPromoted()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new WriterLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new UpgradeableReadLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
        [Test]
        public void TestLockUpgradeableNotPromotedvsUpgradeableNotPromoted()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new UpgradeableReadLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new UpgradeableReadLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
        [Test]
        public void TestLockUpgradeablePromotedvsRead()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new UpgradeableReadLock(lockSlim))
                {
                    using (new WriterLock(lockSlim))
                    {
                        manualResetEvent.Set();
                        Thread.Sleep(timeout);
                        finished = true;
                    }
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new ReaderLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
        [Test]
        public void TestLockReadvsUpgradeablePromoted()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new ReaderLock(lockSlim))
                {
                    manualResetEvent.Set();
                    Thread.Sleep(timeout);
                    finished = true;
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new UpgradeableReadLock(lockSlim))
            {
                Assert.That(finished, Is.False);

                using (new WriterLock(lockSlim))
                {
                    Assert.That(finished, Is.True);
                }
            }
        }
        [Test]
        public void TestLockUpgradeablePromotedvsWrite()
        {
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool finished = false;

            ThreadPool.QueueUserWorkItem(o =>
            {
                using (new UpgradeableReadLock(lockSlim))
                {
                    using (new WriterLock(lockSlim))
                    {
                        manualResetEvent.Set();
                        Thread.Sleep(timeout);
                        finished = true;
                    }
                }
            });

            bool started = manualResetEvent.WaitOne(timeout);
            Assert.That(started, Is.True);

            using (new WriterLock(lockSlim))
            {
                Assert.That(finished, Is.True);
            }
        }
    }
}
