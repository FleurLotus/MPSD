namespace Common.WPF.UnitTests
{
    using System;
    using System.Windows;

    using Common.WPF.Converter;

    using NUnit.Framework;

    [TestFixture]
    public class ProgressBarFillToRectConverterTest
    {
        [Test]
        public void TestConvertWrongParameter()
        {
            ProgressBarFillToRectConverter converter = new ProgressBarFillToRectConverter();
            object ret = converter.Convert(null, null, null, null);
            Assert.That(ret, Is.EqualTo(new Rect(0, 0, 0, 0)));
        }
        [Test]
        public void TestConvert()
        {
            double current = 2.5;
            double max = 10;
            double width = 150;
            double height = 200;

            ProgressBarFillToRectConverter converter = new ProgressBarFillToRectConverter();
            object ret = converter.Convert(new object[] { current, max, width, height }, null, null, null);
            Assert.That(ret, Is.EqualTo(new Rect(0, 0, width * current / max, height)));
        }
        [Test]
        public void TestNoConvertBack()
        {
            ProgressBarFillToRectConverter converter = new ProgressBarFillToRectConverter();
            Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, null));
        }
        [Test]
        public void TestMarkup()
        {
            ProgressBarFillToRectConverter converter = new ProgressBarFillToRectConverter();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}