
namespace Compression.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Compression.Transformation;

    using NUnit.Framework;

    [TestFixture]
    public class BurrowsWheelerTest : TransformationTest
    {
        public BurrowsWheelerTest()
            : base(new BurrowsWheeler())
        {
        }
        
        [Test]
        public void TooLongInputTransformTest()
        {
            Assert.Throws<IndexOutOfRangeException>(() => Transfomation.Transform(new MemoryStream(new byte[BurrowsWheeler.MaxIteration + 1])), "Too long input should throw IndexOutOfRangeException");
        }
        [Test]
        public void TooLongInputReverseTransformTest()
        {
            Assert.Throws<IndexOutOfRangeException>(() => Transfomation.ReverseTransform(new MemoryStream(new byte[BurrowsWheeler.MaxIteration + 3])), "Too long input should throw IndexOutOfRangeException");
        }
        [Test]
        public void WrongFormatReverseTransformTest()
        {
            Assert.Throws<WrongFormattedInputException>(() => Transfomation.ReverseTransform(new MemoryStream(new byte[] { 5, 0, 0 })), "wrong index should throw WrongFormattedInputException");
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void TestTransformation(byte[] source, byte[] expectedResult)
        {
            Stream ms = Transfomation.Transform(new MemoryStream(source));

            Assert.IsNotNull(ms, "ms is null");
            byte[] ret = new byte[ms.Length];
            ms.Read(ret, 0, (int)ms.Length);
            Assert.IsNotNull(ret, "ret is null");
            Assert.IsTrue(ret.Length == source.Length + 2, "Length change during transformation");

            Assert.IsTrue(Compare.ByteArrayValueEquals(expectedResult, ret), "Not the expected value after transform");
            
            ms = Transfomation.ReverseTransform(new MemoryStream(expectedResult));

            Assert.IsNotNull(ms, "ms is null");
            ret = new byte[ms.Length];
            ms.Read(ret, 0, (int)ms.Length);
            Assert.IsNotNull(ret, "ret is null");
            Assert.IsTrue(ret.Length == expectedResult.Length - 2, "Length change during reversetransformation");

            Assert.IsTrue(Compare.ByteArrayValueEquals(source, ret), "Not the expected value after reversetransform");
        }
        public static IEnumerable<object[]> TestCases()
        {
            byte[] source = new byte[256];
            byte[] expected = new byte[258];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)i;
                expected[i + 2] = i == 0 ? (byte)255 : source[i - 1];
            }

            yield return new object[] { source, expected };

            source = new byte[256];
            expected = new byte[258];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)(255 - i);
                expected[i + 2] = i == 255 ? (byte)0 : (byte)(i + 1);
            }
            expected[0] = 255;
            expected[1] = 0;
            yield return new object[] { source, expected };

            yield return new object[] { Encoding.ASCII.GetBytes("abbbaabbbbaccabbaaabc"), new byte[] { 4, 0, 98, 98, 97, 99, 99, 97, 97, 98, 98, 98, 98, 97, 98, 98, 97, 98, 97, 97, 99, 98, 97 } };
        }
    }
}
