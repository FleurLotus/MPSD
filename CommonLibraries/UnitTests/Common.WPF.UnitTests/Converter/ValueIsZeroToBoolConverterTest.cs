namespace Common.WPF.UnitTests
{
    using System;

    using Common.WPF.Converter;

    using NUnit.Framework;

    [TestFixture]
    public class ValueIsZeroToBoolConverterTest
    {
        [Test]
        public void TestConvertZero()
        {
            ValueIsZeroToBoolConverter converter = new ValueIsZeroToBoolConverter();
            object ret = converter.Convert(0, null, null, null);
            Assert.That(ret, Is.True);
        }
        [Test]
        public void TestConvertNoneZero()
        {
            ValueIsZeroToBoolConverter converter = new ValueIsZeroToBoolConverter();
            object ret = converter.Convert(1, null, null, null);
            Assert.That(ret, Is.False);
        }
        [Test]
        public void TestNoConvertBack()
        {
            ValueIsZeroToBoolConverter converter = new ValueIsZeroToBoolConverter();
            Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, null));
        }
        [Test]
        public void TestMarkup()
        {
            ValueIsZeroToBoolConverter converter = new ValueIsZeroToBoolConverter();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}