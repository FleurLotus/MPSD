namespace Common.SQL.UnitTests
{
    using System;
    using System.Data;

    using NUnit.Framework;
    using Moq;

    [TestFixture]
    public class DataRecordExtDataRecordExtTest
    {
        private IDataRecord _dataRecord;

        [SetUp]
        public void Setup()
        {
            Mock<IDataRecord> mock = new Mock<IDataRecord>(MockBehavior.Strict);

            mock.Setup(d => d.IsDBNull(It.IsAny<int>())).Returns(true);
            mock.Setup(d => d.IsDBNull(1)).Returns(false);
            mock.Setup(d => d.GetBoolean(1)).Returns(true);
            mock.Setup(d => d.GetString(1)).Returns("aaaa");
            mock.Setup(d => d.GetInt16(1)).Returns(16);
            mock.Setup(d => d.GetInt32(1)).Returns(32);
            mock.Setup(d => d.GetInt64(1)).Returns(64);
            mock.Setup(d => d.GetDouble(1)).Returns(3.1415);
            mock.Setup(d => d.GetDateTime(1)).Returns(new DateTime(2024, 1, 1));
            mock.Setup(d => d.GetByte(1)).Returns(5);
            mock.Setup(d => d.GetChar(1)).Returns('b');

            _dataRecord = mock.Object;
        }
        [Test]
        public void GetStringOrDefaultTest()
        {
            Assert.That(_dataRecord.GetStringOrDefault(0), Is.Null);
            Assert.That(_dataRecord.GetStringOrDefault(1), Is.EqualTo("aaaa"));
        }
        [Test]
        public void GetBoolOrDefaultTest()
        {
            Assert.That(_dataRecord.GetBoolOrDefault(0), Is.False);
            Assert.That(_dataRecord.GetBoolOrDefault(1), Is.True);
        }
        [Test]
        public void GetInt32OrDefaultTest()
        {
            Assert.That(_dataRecord.GetInt32OrDefault(0), Is.EqualTo(0));
            Assert.That(_dataRecord.GetInt32OrDefault(1), Is.EqualTo(32));
        }
        [Test]
        public void GetInt16OrDefaultTest()
        {
            Assert.That(_dataRecord.GetInt16OrDefault(0), Is.EqualTo(0));
            Assert.That(_dataRecord.GetInt16OrDefault(1), Is.EqualTo(16));
        }
        [Test]
        public void GetInt64OrDefaultTest()
        {
            Assert.That(_dataRecord.GetInt64OrDefault(0), Is.EqualTo(0));
            Assert.That(_dataRecord.GetInt64OrDefault(1), Is.EqualTo(64));
        }
        [Test]
        public void GetDoubleOrDefaultTest()
        {
            Assert.That(_dataRecord.GetDoubleOrDefault(0), Is.EqualTo(0.0));
            Assert.That(_dataRecord.GetDoubleOrDefault(1), Is.EqualTo(3.1415));
        }
        [Test]
        public void GetDateTimeOrDefaultTest()
        {
            Assert.That(_dataRecord.GetDateTimeOrDefault(0), Is.EqualTo(new DateTime()));
            Assert.That(_dataRecord.GetDateTimeOrDefault(1), Is.EqualTo(new DateTime(2024, 1, 1)));
        }
        [Test]
        public void GetByteOrDefaultTest()
        {
            Assert.That(_dataRecord.GetByteOrDefault(0), Is.EqualTo(0));
            Assert.That(_dataRecord.GetByteOrDefault(1), Is.EqualTo(5));
        }
        [Test]
        public void GetCharOrDefaultTest()
        {
            Assert.That(_dataRecord.GetCharOrDefault(0), Is.EqualTo(0));
            Assert.That(_dataRecord.GetCharOrDefault(1), Is.EqualTo('b'));
        }
        [Test]
        public void GetStringOrNullTest()
        {
            Assert.That(_dataRecord.GetStringOrNull(0), Is.Null);
            Assert.That(_dataRecord.GetStringOrNull(1), Is.EqualTo("aaaa"));
        }
        [Test]
        public void GetBoolOrNullTest()
        {
            Assert.That(_dataRecord.GetBoolOrNull(0), Is.Null);
            Assert.That(_dataRecord.GetBoolOrNull(1), Is.True);
        }
        [Test]
        public void GetInt16OrNullTest()
        {
            Assert.That(_dataRecord.GetInt16OrNull(0), Is.Null);
            Assert.That(_dataRecord.GetInt16OrNull(1), Is.EqualTo(16));
        }
        [Test]
        public void GetInt32OrNullTest()
        {
            Assert.That(_dataRecord.GetInt32OrNull(0), Is.Null);
            Assert.That(_dataRecord.GetInt32OrNull(1), Is.EqualTo(32));
        }
        [Test]
        public void GetInt64OrNullTest()
        {
            Assert.That(_dataRecord.GetInt64OrNull(0), Is.Null);
            Assert.That(_dataRecord.GetInt64OrNull(1), Is.EqualTo(64));
        }
        [Test]
        public void GetDoubleOrNullTest()
        {
            Assert.That(_dataRecord.GetDoubleOrNull(0), Is.Null);
            Assert.That(_dataRecord.GetDoubleOrNull(1), Is.EqualTo(3.1415));
        }
        [Test]
        public void GetDateTimeOrNullTest()
        {
            Assert.That(_dataRecord.GetDateTimeOrNull(0), Is.Null);
            Assert.That(_dataRecord.GetDateTimeOrNull(1), Is.EqualTo(new DateTime(2024, 1, 1)));
        }
        [Test]
        public void GetByteOrNullTest()
        {
            Assert.That(_dataRecord.GetByteOrNull(0), Is.Null);
            Assert.That(_dataRecord.GetByteOrNull(1), Is.EqualTo(5));
        }
        [Test]
        public void GetCharOrNullTest()
        {
            Assert.That(_dataRecord.GetCharOrNull(0), Is.Null);
            Assert.That(_dataRecord.GetCharOrNull(1), Is.EqualTo('b'));
        }
    }
}
