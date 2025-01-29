namespace Common.WPF.UnitTests
{
    using NUnit.Framework;

    using Common.WPF.Converter;

    [TestFixture]
    public class BoolInvertConverterTest
    {
        [Test]
        public void TestConvertTrue()
        {
            BoolInvertConverter converter = new BoolInvertConverter();
            object ret = converter.Convert(true, null, null, null);
            Assert.That(ret, Is.EqualTo(false));
        }
        [Test]
        public void TestConvertFalse()
        {
            BoolInvertConverter converter = new BoolInvertConverter();
            object ret = converter.Convert(false, null, null, null);
            Assert.That(ret, Is.EqualTo(true));
        }
        [Test]
        public void TestConvertBackTrue()
        {
            BoolInvertConverter converter = new BoolInvertConverter();
            object ret = converter.ConvertBack(true, null, null, null);
            Assert.That(ret, Is.EqualTo(false));
        }
        [Test]
        public void TestConvertBackFalse()
        {
            BoolInvertConverter converter = new BoolInvertConverter();
            object ret = converter.ConvertBack(false, null, null, null);
            Assert.That(ret, Is.EqualTo(true));
        }
        [Test]
        public void TestMarkup()
        {
            BoolInvertConverter converter = new BoolInvertConverter();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}
