
namespace Compression.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;


    using NUnit.Framework;

    [TestFixture]
    public class DeflateTest
    {
       [Test]
        [TestCaseSource("TestCases")]
        public void TestTransformation(byte[] source)
        {
            Stream ret =  Compressor.Deflate(new MemoryStream(source));
            Assert.IsNotNull(ret, "ret is null");

            byte[] tmp = new byte[source.Length];
            Stream ret2 = Compressor.Inflate(ret);
            Assert.IsTrue(ret2.Read(tmp, 0, tmp.Length) == tmp.Length);
            Assert.IsTrue(Compare.ByteArrayValueEquals(source, tmp), "Not the expected value after transform");
        }
        public IEnumerable<object[]> TestCases()
        {
            byte[] source = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)i;
            }

            yield return new object[] { source };
            
            source = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)(255 - i);
            }
            
            yield return new object[] { source};
            yield return new object[] { Encoding.ASCII.GetBytes("abbbaabbbbaccabbaaabc")};
            yield return new object[] { Encoding.ASCII.GetBytes("aaaaaaaaaaaaa")};


            FileStream fs = new FileStream(@"..\..\..\..\MagicPictureSetDownloader\ExternalReference\Xceed.Wpf.Toolkit.dll", FileMode.Open);
            byte[] temp = new byte[fs.Length];
            //byte[] temp = new byte[65000];
            fs.Read(temp, 0, temp.Length);
            fs.Close();

            yield return new object[] { temp };
        }
    }
}
