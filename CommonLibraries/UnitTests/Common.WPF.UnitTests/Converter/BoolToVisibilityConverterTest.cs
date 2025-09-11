namespace Common.WPF.UnitTests
{
    using System;
    using System.Windows;

    using Common.WPF.Converter;

    using NUnit.Framework;

    [TestFixture]
    public class BoolToVisibilityConverterTest
    {
        [Test]
        public void TestConvertTrue()
        {
            BoolToVisibilityConverter converter = new BoolToVisibilityConverter();
            object ret = converter.Convert(true, null, null, null);
            Assert.That(ret, Is.EqualTo(Visibility.Visible));
        }
        [Test]
        public void TestConvertFalse()
        {
            BoolToVisibilityConverter converter = new BoolToVisibilityConverter();
            object ret = converter.Convert(false, null, null, null);
            Assert.That(ret, Is.EqualTo(Visibility.Collapsed));
        }
        [Test]
        public void TestNoConvertBack()
        {
            BoolToVisibilityConverter converter = new BoolToVisibilityConverter();
            Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, null));
        }
        [Test]
        public void TestMarkup()
        {
            BoolToVisibilityConverter converter = new BoolToVisibilityConverter();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}