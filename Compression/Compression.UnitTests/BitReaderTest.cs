
namespace Compression.UnitTests
{
    using System;
    using System.IO;

    using Compression.IO;

    using NUnit.Framework;

    [TestFixture]
    public class BitReaderTest
    {
        [Test]
        public void NullArgsConstructorTest()
        {
            Assert.Throws<ArgumentNullException>(() => new BitReader(null), "null args should throw ArgumentNullException");
        }
        [Test]
        public void EmptyArgsConstructorTest()
        {
            Assert.Throws<ArgumentNullException>(() => new BitReader(new MemoryStream(new byte[0])), "empty args should throw ArgumentNullException");
        }
        [Test]
        public void NotEmptyArgsConstructorTest()
        {
            Assert.IsNotNull(new BitReader(new MemoryStream(new byte[] { 0x1 })), "instance is null");
        }
        [Test]
        public void EoFTest()
        {
            BitReader bitReader = new BitReader(new MemoryStream(new byte[] { 0xFF, 0xFF }));
            Assert.IsNotNull(bitReader, "instance is null");

            for (int i = 0; i < 16; i++)
            {
                Assert.IsFalse(bitReader.EoF, "EoF should be false for iteration " + i);
                bitReader.ReadNext();
            }
            Assert.IsTrue(bitReader.EoF, "EoF should be true for iteration 16");

            Assert.Throws<IndexOutOfRangeException>(() => bitReader.ReadNext(), "ReadNext should throw IndexOutOfRangeException when at EoF");
        }

        [Test]
        public void ValueReadingTest()
        {
            
            BitReader bitReader = new BitReader(new MemoryStream(new byte[] { 0xF0, 0x5A }));
            Assert.IsNotNull(bitReader, "instance is null");

            //F0  = 1111 0000
            for (int i = 0; i < 4; i++)
                Assert.IsTrue(bitReader.ReadNext(), "Read value should be true for i=" + i);

            for (int j = 0; j < 4; j++)
                Assert.IsFalse(bitReader.ReadNext(), "Read value should be false for j=" + j);

            //5A = 0101 1010
            for (int k = 0; k < 2; k++)
            {
                Assert.IsFalse(bitReader.ReadNext(), "Read value should be false for k=" + k);
                Assert.IsTrue(bitReader.ReadNext(), "Read value should be true for step 2  of k=" + k);
            }

            for (int l = 0; l < 2; l++)
            {
                Assert.IsTrue(bitReader.ReadNext(), "Read value should be true for of l=" + l);
                Assert.IsFalse(bitReader.ReadNext(), "Read value should be false for step 2 l=" + l);
            }
        }
        [Test]
        public void ByteReadingTest()
        {

            BitReader bitReader = new BitReader(new MemoryStream(new byte[] { 0xF0, 0x5A, 0xCE, 0x00 }));
            Assert.IsNotNull(bitReader, "instance is null");

            Assert.IsTrue(bitReader.ReadByte() == 0xF0, "Wrong read value");
            Assert.IsFalse(bitReader.ReadNext(), "Read value should be false");
            Assert.IsTrue(bitReader.ReadByte() == 0xB5, "Wrong read value");
            Assert.IsTrue(bitReader.ReadByte() == 0x9C, "Wrong read value");
            Assert.Throws<IndexOutOfRangeException>(() => bitReader.ReadByte(), "ReadByte should throw IndexOutOfRangeException when at EoF");
       }
    }
}
