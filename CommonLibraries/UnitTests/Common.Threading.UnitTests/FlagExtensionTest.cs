namespace Common.Threading.UnitTests
{
    using System;

    using Common.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class FlagExtensionTest
    {
        private const string key = "aaa";

        [Test]
        public void SetFlagNullObject()
        {
            object obj = null;
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("source"), () => obj.SetFlag(key));
        }
        [Test]
        public void SetFlagNullArg()
        {
            object obj = new object();
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Message.StartsWith("Can't be null or empty").And.Property("ParamName").EqualTo("name"), () => obj.SetFlag(null));
        }
        [Test]
        public void SetFlagEmptyArg()
        {
            object obj = new object();
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Message.StartsWith("Can't be null or empty").And.Property("ParamName").EqualTo("name"), () => obj.SetFlag(""));
        }
        [Test]
        public void IsFlagSetNullObject()
        {
            object obj = null;
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("source"), () => obj.IsFlagSet(key));
        }
        [Test]
        public void IsFlagSetNullArg()
        {
            object obj = new object();
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Message.StartsWith("Can't be null or empty").And.Property("ParamName").EqualTo("name"), () => obj.IsFlagSet(null));
        }
        [Test]
        public void IsFlagSetEmptyArg()
        {
            object obj = new object();
            Assert.Throws(Is.TypeOf<ArgumentException>().With.Message.StartsWith("Can't be null or empty").And.Property("ParamName").EqualTo("name"), () => obj.IsFlagSet(""));
        }
        [Test]
        public void TestFlagDispose()
        {
            object obj = new object();
            IDisposable flag = obj.SetFlag(key);
            flag.Dispose();
            Assert.Throws<ObjectDisposedException>(() => flag.Dispose());
        }
        [Test]
        public void TestBasicFlag()
        {
            object obj = new object();
            Assert.That(obj.IsFlagSet(key), Is.False);
            using (IDisposable flag = obj.SetFlag(key))
            {
                Assert.That(obj.IsFlagSet(key), Is.True);
            }
            Assert.That(obj.IsFlagSet(key), Is.False);
        }
        [Test]
        public void TestMultiSetFlagSameFlag()
        {
            object obj = new object();
            Assert.That(obj.IsFlagSet(key), Is.False);
            using (IDisposable flag = obj.SetFlag(key))
            {
                Assert.That(obj.IsFlagSet(key), Is.True);
                using (IDisposable flag2 = obj.SetFlag(key))
                {
                    Assert.That(obj.IsFlagSet(key), Is.True);
                }
                Assert.That(obj.IsFlagSet(key), Is.True);
            }
            Assert.That(obj.IsFlagSet(key), Is.False);
        }
        [Test]
        public void TestMultiSetFlagDifferentKey()
        {
            object obj = new object();
            string key2 = "bbb";

            Assert.That(obj.IsFlagSet(key), Is.False);
            Assert.That(obj.IsFlagSet(key2), Is.False);
            using (IDisposable flag = obj.SetFlag(key))
            {
                Assert.That(obj.IsFlagSet(key), Is.True);
                Assert.That(obj.IsFlagSet(key2), Is.False);
                using (IDisposable flag2 = obj.SetFlag(key2))
                {
                    Assert.That(obj.IsFlagSet(key), Is.True);
                    Assert.That(obj.IsFlagSet(key2), Is.True);
                }
                Assert.That(obj.IsFlagSet(key), Is.True);
                Assert.That(obj.IsFlagSet(key2), Is.False);
            }
            Assert.That(obj.IsFlagSet(key), Is.False);
            Assert.That(obj.IsFlagSet(key2), Is.False);
        }
        [Test]
        public void TestMultiSetFlagDifferentObject()
        {
            object obj = new object();
            object obj2 = new object();

            Assert.That(obj.IsFlagSet(key), Is.False);
            Assert.That(obj2.IsFlagSet(key), Is.False);
            using (IDisposable flag = obj.SetFlag(key))
            {
                Assert.That(obj.IsFlagSet(key), Is.True);
                Assert.That(obj2.IsFlagSet(key), Is.False);
                using (IDisposable flag2 = obj2.SetFlag(key))
                {
                    Assert.That(obj.IsFlagSet(key), Is.True);
                    Assert.That(obj2.IsFlagSet(key), Is.True);
                }
                Assert.That(obj.IsFlagSet(key), Is.True);
                Assert.That(obj2.IsFlagSet(key), Is.False);
            }
            Assert.That(obj.IsFlagSet(key), Is.False);
            Assert.That(obj2.IsFlagSet(key), Is.False);
        }
        [Test]
        public void TestFlagCount()
        {
            object obj = new object();
            FlagCount flag = new FlagCount(key);
            Assert.That(flag.Name, Is.EqualTo(key));
            Assert.Throws<Exception>(() => flag.Decrement(obj));
        }
    }
}