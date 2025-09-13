namespace Common.WPF.Converter.UnitTests
{
    using System;
    using System.Windows.Data;

    using Common.WPF.Converter;

    using NUnit.Framework;

    [TestFixture]
    public class AggregateConverterTest
    {
        [Test]
        public void TestConvert2()
        {
            IValueConverter conv = new BoolInvertConverter();

            AggregateConverter converter = new AggregateConverter(conv, conv);
            object ret = converter.Convert(true, null, null, null);
            Assert.That(ret, Is.EqualTo(true));
        }
        [Test]
        public void TestConvert3()
        {
            IValueConverter conv = new BoolInvertConverter();

            AggregateConverter converter = new AggregateConverter(conv, conv, conv);
            object ret = converter.Convert(true, null, null, null);
            Assert.That(ret, Is.EqualTo(false));
        }
        [Test]
        public void TestConvert4()
        {
            IValueConverter conv = new BoolInvertConverter();

            AggregateConverter converter = new AggregateConverter(conv, conv, conv, conv);
            object ret = converter.Convert(true, null, null, null);
            Assert.That(ret, Is.EqualTo(true));
        }
        [Test]
        public void TestNoConvertBack()
        {
            AggregateConverter converter = new AggregateConverter(null, null);
            Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, null));
        }
        [Test]
        public void TestMarkup()
        {
            AggregateConverter converter = new AggregateConverter(null, null);
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}