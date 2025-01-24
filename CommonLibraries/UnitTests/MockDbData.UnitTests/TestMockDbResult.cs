namespace MockDbData.UnitTests
{
    using System;
    using System.Data;

    using NUnit.Framework;

    [TestFixture]
    public class TestMockDbResult
    {
        [Test]
        public void TestConstuctor()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("table"), () => new MockDbResult((DataTable)null), "null arg should throw ArgumentNullException");
            Assert.DoesNotThrow(() => new MockDbResult(new DataTable()), "None null DataTable should not throw");
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("tables"), () => new MockDbResult((DataTable[])null), "null arg should throw ArgumentNullException");
            Assert.DoesNotThrow(() => new MockDbResult(new DataTable[] { new DataTable() }), "None null DataTable should not throw");
            Assert.DoesNotThrow(() => new MockDbResult(new DataTable[] { new DataTable(), new DataTable() }), "None null DataTable should not throw");
            Assert.Throws(Is.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("tables"), () => new MockDbResult(new DataTable[] { new DataTable(), null, new DataTable() }), "Any null DataTable arg should throw");
        }
    }
}