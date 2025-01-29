namespace Common.WPF.UnitTests
{
    using NUnit.Framework;

    using Common.WPF.Converter;

    [TestFixture]
    public class EnumMatchToBooleanConverterTest
    {
        private enum TestEnum
        {
            Value1,
            Value2,
            Value3,
            Value4,
            Value5,
        }

        [Test]
        public void TestConvertNoValue()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.Convert(null, null, "", null);
            Assert.That(ret, Is.EqualTo(false));
        }
        [Test]
        public void TestConvertNoParameter()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.Convert("", null, null, null);
            Assert.That(ret, Is.EqualTo(false));
        }
        [Test]
        public void TestConvertInList()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.Convert(TestEnum.Value2, null, "Value1@Value2@Value3", null);
            Assert.That(ret, Is.EqualTo(true));
        }
        [Test]
        public void TestConvertNotInList()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.Convert(TestEnum.Value4, null, "Value1@Value2@Value3", null);
            Assert.That(ret, Is.EqualTo(false));
        }
        [Test]
        public void TestConvertBackNoValue()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.ConvertBack(null, null, "", null);
            Assert.That(ret, Is.Null);
        }
        [Test]
        public void TestConvertBackNoParameter()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.ConvertBack("", null, null, null);
            Assert.That(ret, Is.Null);
        }
        [Test]
        public void TestConvertBackFromList()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.ConvertBack(true, typeof(TestEnum), "Value1@Value2@Value3", null);
            Assert.That(ret, Is.Null);
        }
        [Test]
        public void TestConvertBackFromValueTrue()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.ConvertBack(true, typeof(TestEnum), "Value2", null);
            Assert.That(ret, Is.EqualTo(TestEnum.Value2));
        }
        [Test]
        public void TestConvertBackFromValueFalse()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            object ret = converter.ConvertBack(false, typeof(TestEnum), "Value2", null);
            Assert.That(ret, Is.Null);
        }
        [Test]
        public void TestMarkup()
        {
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            Assert.That(converter.ProvideValue(null), Is.EqualTo(converter));
        }
    }
}
