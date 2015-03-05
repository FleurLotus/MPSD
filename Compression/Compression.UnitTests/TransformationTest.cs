
namespace Compression.UnitTests
{
    using System.IO;

    using NUnit.Framework;

    public abstract class TransformationTest
    {
        protected readonly ITransformation Transfomation;

        protected TransformationTest(ITransformation transfomation)
        {
            Transfomation = transfomation;
        }

        [Test]
        public void NullArgsTransformationTest()
        {
            Assert.IsNull(Transfomation.Transform(null), "null args should return null");
        }
        [Test]
        public void EmptyArgsTransformationTest()
        {
            Assert.IsTrue(Transfomation.Transform(new MemoryStream(new byte[0])).Length == 0, "empty args should return [0]");
        }
        [Test]
        public void NullArgsReverseTransformationTest()
        {
            Assert.IsNull(Transfomation.ReverseTransform(null), "null args should return null");
        }
        [Test]
        public void EmptyArgsReverseTransformationTest()
        {
            Assert.IsTrue(Transfomation.ReverseTransform(new MemoryStream(new byte[0])).Length == 0, "empty args should return [0]");
        }
    }
}
