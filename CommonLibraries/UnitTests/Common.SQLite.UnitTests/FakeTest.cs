namespace Common.SQLite.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class FakeTest
    {
        [Test]
        public void TestDefault()
        {
            Assert.That(null, Is.Null);
        }
    }
}
