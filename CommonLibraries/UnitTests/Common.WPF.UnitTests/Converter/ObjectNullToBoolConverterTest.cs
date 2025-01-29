namespace Common.WPF.UnitTests
{
    using System;

    using NUnit.Framework;

    using Common.WPF.Converter;

    [TestFixture]
    public class ObjectNullToBoolConverterTest
    {
        [Test]
        public void TestConvertNull()
        {
            ObjectNullToBoolConverter converter = new ObjectNullToBoolConverter();
            object ret = converter.Convert(null, null, null, null);
            Assert.That(ret, Is.True);
        }
        [Test]
        public void TestConvertNotNull()
        {
            ObjectNullToBoolConverter converter = new ObjectNullToBoolConverter();
            object ret = converter.Convert(new object(), null, null, null);
            Assert.That(ret, Is.False);
        }
        [Test]
        public void TestNoConvertBack()
        {
            ObjectNullToBoolConverter converter = new ObjectNullToBoolConverter();
            Assert.Throws<NotImplementedException>(()=> converter.ConvertBack(null, null, null, null));
        }
        [Test]
        public void TestMarkup()
        {
            ObjectNullToBoolConverter converter = new ObjectNullToBoolConverter();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}
