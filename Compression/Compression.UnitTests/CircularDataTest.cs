namespace Compression.UnitTests
{
    using System;
    using System.Collections.Generic;

    using Compression;

    using NUnit.Framework;

    [TestFixture]
    public class CircularDataTest
    {
        [Test]
        public void NullArgsConstructorTest()
        {
            Assert.Throws<ArgumentNullException>(() => new CircularData(null, 0), "null args should throw ArgumentNullException");
        }
        [Test]
        public void EmptyArgsConstructorTest()
        {
            Assert.Throws<ArgumentNullException>(() => new CircularData(new byte[0], 0), "empty args should throw ArgumentNullException");
        }
        [Test]
        public void NegativeIndexArgsConstructorTest()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new CircularData(new byte[] { 0x1 }, -1), "negative index should throw IndexOutOfRangeException");
        }
        [Test]
        public void TooBigIndexArgsConstructorTest()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new CircularData(new byte[] { 0x1 }, 2), "too big index should throw IndexOutOfRangeException");
        }
        [Test]
        public void NotEmptyArgsConstructorTest()
        {
            Assert.IsNotNull(new CircularData(new byte[] { 0x1 }, 0), "instance is null");
        }
        [Test]
        [TestCaseSource("TestCases")]
        public void TestCompaison(byte[] source1, int index1, byte[] source2, int index2, int expectedResult )
        {
            CircularData circularData1 = new CircularData(source1, index1);
            CircularData circularData2 = new CircularData(source2, index2);

            Assert.AreEqual(circularData1.CompareTo(circularData2), expectedResult);
        }
        public IEnumerable<object[]> TestCases()
        {
            byte[] source1 = { 1, 2, 3, 1, 2, 3 };
            byte[] source2 = { 1, 2, 3 };
            
            //Simple value test
            yield return new object[] { source1, 0, source1, 1, -1 };
            //Rotation test
            yield return new object[] { source1, 1, source1, 4, 0 };
            //Length test 
            yield return new object[] { source1, 0, source2, 0, 1 };
            //Length test with rotation
            yield return new object[] { source1, 3, source2, 0, 1 };
        }
    }
}
