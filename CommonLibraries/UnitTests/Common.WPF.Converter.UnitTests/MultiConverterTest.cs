namespace Common.WPF.Converter.UnitTests
{
    using System;
    using System.Globalization;

    using Common.WPF.Converter;

    using NUnit.Framework;

    [TestFixture]
    public class MultiConverterTest
    {
        public class MultiConverterDummy : MultiConverter
        {
            public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
            public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestMarkup()
        {
            MultiConverterDummy converter = new MultiConverterDummy();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}