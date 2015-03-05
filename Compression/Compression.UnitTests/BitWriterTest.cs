namespace Compression.UnitTests
{
    using Compression.IO;

    using NUnit.Framework;

    [TestFixture]
    public class BitWriterTest
    {
        [Test]
        public void LengthTest()
        {
            BitWriter bitWriter = new BitWriter();
            Assert.IsNotNull(bitWriter, "instance is null");

            Assert.IsTrue(bitWriter.Length == 0, "Length should be 0");

            for (int i = 1; i <= 1000; i++)
            {
                bitWriter.Write(i % 5 == 0);
                Assert.IsTrue(bitWriter.Length == i, "Length should be " + i);
            }
        }
        [Test]
        public void ValuesTest()
        {
            BitWriter bitWriter = new BitWriter();
            Assert.IsNotNull(bitWriter, "instance is null");

            Assert.IsTrue(bitWriter.Length == 0, "Length should be 0");
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[0]), "Values should be []");

            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0x80 }), "Values should be [1000 0000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC0 }), "Values should be [1100 0000]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC0 }), "Values should be [1100 0000]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC0 }), "Values should be [1100 0000]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC0 }), "Values should be [1100 0000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC4 }), "Values should be [1100 0100]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC4 }), "Values should be [1100 0100]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5 }), "Values should be [1100 0101]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x0 }), "Values should be [1100 0101, 0000 0000]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x0 }), "Values should be [1100 0101, 0000 0000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x20 }), "Values should be [1100 0101, 0010 0000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x30 }), "Values should be [1100 0101, 0011 0000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x38 }), "Values should be [1100 0101, 0011 1000]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x38 }), "Values should be [1100 0101, 0011 1000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x3A }), "Values should be [1100 0101, 0011 1010]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC5, 0x3A }), "Values should be [1100 0101, 0011 1010]");
        }
        [Test]
        public void MultiWriteTest()
        {
            BitWriter bitWriter = new BitWriter();
            Assert.IsNotNull(bitWriter, "instance is null");

            Assert.IsTrue(bitWriter.Length == 0, "Length should be 0");
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[0]), "Values should be []");

            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0x80 }), "Values should be [1000 0000]");
            bitWriter.Write(new[] { true, false, false, false, true, true, false, true, false, true, true, true, true, false, false, true, false });
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC6, 0xBC, 0x80 }), "Values should be [1100 0110, 1011 1100, 1000 0000]");
            bitWriter.Write(new[] { true, false, false, false, true, true, false, true, false, true, true, true, true, false, false, true, false });
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xC6, 0xBC, 0xA3, 0x5E, 0x40 }), "Values should be [1100 0110, 1011 1100, 1010 0011, 0101 1110, 0100 0000]");
        }
        [Test]
        public void ByteWriteTest()
        {
            BitWriter bitWriter = new BitWriter();
            Assert.IsNotNull(bitWriter, "instance is null");

            Assert.IsTrue(bitWriter.Length == 0, "Length should be 0");
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[0]), "Values should be []");

            bitWriter.Write(0xFA);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA }), "Values should be [1111 1010]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x00 }), "Values should be [1111 1010, 0000 0000]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x00 }), "Values should be [1111 1010, 0000 0000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x20 }), "Values should be [1111 1010, 0010 0000]");
            bitWriter.Write(true);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x30 }), "Values should be [1111 1010, 0011 0000]");
            bitWriter.Write(0xFF);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x3F, 0xF0 }), "Values should be [1111 1010, 0011 1111, 1111 0000]");
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x3F, 0xF0 }), "Values should be [1111 1010, 0011 1111, 1111 0000]");
            bitWriter.Write(0xC3);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x3F, 0xF6, 0x18 }), "Values should be [1111 1010, 0011 1111, 1111 0110, 0001 1000]");
            bitWriter.Write(new byte[] {0xC3, 0x81});
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x3F, 0xF6, 0x1E, 0x1C, 0x08 }), "Values should be [1111 1010, 0011 1111, 1111 0110, 0001 1110, 0001 1100, 0000 1000]");
            bitWriter.Write(false);
            bitWriter.Write(false);
            bitWriter.Write(false);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x3F, 0xF6, 0x1E, 0x1C, 0x08 }), "Values should be [1111 1010, 0011 1111, 1111 0110, 0001 1110, 0001 1100, 0000 1000]");
            bitWriter.Write(0xFF);
            Assert.IsTrue(Compare.ByteArrayValueEquals(bitWriter.GetValues(), new byte[] { 0xFA, 0x3F, 0xF6, 0x1E, 0x1C, 0x08, 0xFF }), "Values should be [1111 1010, 0011 1111, 1111 0110, 0001 1110, 0001 1100, 0000 1000, 1111 1111]");
        }
    }
}
