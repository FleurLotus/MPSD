
namespace Compression.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Compression.Transformation;

    using NUnit.Framework;

    [TestFixture]
    public class MoveToFrontTest: TransformationTest
    {
        public MoveToFrontTest()
            : base(new MoveToFront())
        {
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
            Assert.IsTrue(ret.Length == source.Length, "Length change during transformation");

            Assert.IsTrue(Compare.ByteArrayValueEquals(expectedResult, ret), "Not the expected value after transform");

            ms = Transfomation.ReverseTransform(new MemoryStream(expectedResult));

            Assert.IsNotNull(ms, "ms is null");
            ret = new byte[ms.Length];
            ms.Read(ret, 0, (int)ms.Length);
            Assert.IsNotNull(ret, "ret is null");
            Assert.IsTrue(ret.Length == expectedResult.Length, "Length change during reversetransformation");

            Assert.IsTrue(Compare.ByteArrayValueEquals(source, ret), "Not the expected value after reversetransform");
        }
        public static IEnumerable<object[]> TestCases()
        {
            byte[] source = new byte[256];
            byte[] expected = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)i;
                expected[i] = source[i];
            }

            yield return new object[] { source, expected };

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)(255 - i);
                expected[i] = 255;
            }
            yield return new object[] { source, expected };

            yield return new object[] { Encoding.ASCII.GetBytes("abbbaabbbbaccabbaaabc"), new byte[] { 97, 98, 0, 0, 1, 0, 1, 0, 0, 0, 1, 99, 0, 1, 2, 0, 1, 0, 0, 1, 2 } };
        }
    }
}
