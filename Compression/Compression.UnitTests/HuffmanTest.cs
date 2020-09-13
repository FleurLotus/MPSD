
namespace Compression.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Compression.Transformation;

    using NUnit.Framework;

    [TestFixture]
    public class HuffmanTest : TransformationTest
    {
        public HuffmanTest()
            : base(new Huffman())
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
            Assert.IsTrue(Compare.ByteArrayValueEquals(expectedResult, ret), "Not the expected value after transform");

            ms = Transfomation.ReverseTransform(new MemoryStream(expectedResult));

            Assert.IsNotNull(ms, "ms is null");
            ret = new byte[ms.Length];
            ms.Read(ret, 0, (int)ms.Length);
            Assert.IsNotNull(ret, "ret is null");
            Assert.IsTrue(Compare.ByteArrayValueEquals(source, ret), "Not the expected value after reversetransform");
        }
        public static IEnumerable<object[]> TestCases()
        {
            byte[] source = new byte[256];
            byte[] expected = new byte[259];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)i;
                expected[i + 3] = source[i];
            }
            expected[0] = 0x00;
            expected[1] = 0x00;
            expected[2] = 0x01;

            yield return new object[] { source, expected };
            
            source = new byte[256];
            expected = new byte[259];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)(255 - i);
                expected[i + 3] = source[i];
            }
            expected[0] = 0x00;
            expected[1] = 0x00;
            expected[2] = 0x01;
            yield return new object[] { source, expected };
            
            yield return new object[] { Encoding.ASCII.GetBytes("abbbaabbbbaccabbaaabc"), new byte[] { 0x01, 0x15, 0x00, 0x58, 0x96, 0x3B, 0x0E, 0x3C, 0x3A, 0xCF, 0xD0 } };
            yield return new object[] { Encoding.ASCII.GetBytes("aaaaaaaaaaaaa"), new byte[] { 0x01, 0x0D, 0x00, 0x58, 0x6C, 0x20, 0x00 } };
        }
    }
}
